using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class QueryableExtensions
    {
        public static async Task<QueryResult<TResult>> QueryAsync<TSource, TResult>(
            this IQueryable<TSource> source,
            QueryParam queryParam,
            Expression<Func<TSource, TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            var query = source
                .Select(selector)
                .Where(queryParam.Filter);

            return new QueryResult<TResult>()
            {
                ItemsCount = await query.LongCountAsync(cancellationToken),
                Rows = await query.Pagination(queryParam).ToListAsync(cancellationToken)
            };
        }

        public static IQueryable<TSource> Pagination<TSource>(this IQueryable<TSource> source, IPagination args)
        {
            var page = Math.Max(1, args.Page) - 1;
            var rows = Math.Max(1, args.Rows);

            return source
                .OrderBy(args.OrderBy)
                .Skip(page * rows)
                .Take(rows);
        }

        public static IQueryable<TSource> WhereIn<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IEnumerable<TKey> values)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(keySelector);
            ArgumentNullException.ThrowIfNull(values);

            if (!values.Any())
            {
                return source.Where(x => false);
            }

            var parameter = keySelector.Parameters.Single();
            var equals = values.Select(value => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(value, typeof(TKey))));
            var body = equals.Aggregate(Expression.OrElse);
            var lambda = Expression.Lambda<Func<TSource, bool>>(body, parameter);

            return source.Where(lambda);
        }

        public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, IDictionary<string, SortType> sortedColumns)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (sortedColumns is null)
            {
                return source;
            }

            var index = 0;
            var orderBy = sortedColumns
                .Aggregate(source.Expression, (src, pair) => CreateOrderBy(src, pair.Key, pair.Value, index++));

            if (orderBy.CanReduce)
            {
                orderBy = orderBy.Reduce();
            }
            return source.Provider.CreateQuery<TSource>(orderBy);
        }

        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, FilterRule filter)
        {
            if (filter == null)
            {
                return source;
            }

            var parameterType = source.ElementType;
            var parameter = Expression.Parameter(parameterType, "x");
            var filterExpression = BuildFilterExpressionTree(parameter, filter);
            var predicate = Expression.Lambda(filterExpression, parameter);
            var caller = Expression.Call(
                typeof(Queryable),
                nameof(Queryable.Where),
                [parameterType],
                source.Expression,
                predicate);

            return source.Provider.CreateQuery<TSource>(caller);
        }

        private static MethodCallExpression CreateOrderBy(Expression source, string propertyName, SortType sortType, int index)
        {
            var sourceType = source.Type.GenericTypeArguments[0];
            var parameter = Expression.Parameter(sourceType, "x");
            var property = propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
            var keySelector = Expression.Lambda(property, parameter);
            var method = (sortType == SortType.DESC) ?
                    (index == 0) ? nameof(Queryable.OrderByDescending) : nameof(Queryable.ThenByDescending) :
                    (index == 0) ? nameof(Queryable.OrderBy) : nameof(Queryable.ThenBy);

            //source.OrderByDescending<TSource, int>(e => e.ProperyName);
            var result = Expression.Call(typeof(Queryable), method, [sourceType, property.Type], source, keySelector);
            return result;
        }

        private static Expression BuildFilterExpressionTree(Expression parameter, FilterRule filter)
        {
            Expression expression = null;

            if (filter.Rules != null && filter.Rules.Count > 0)
            {
                expression = filter.Rules
                    .Select(rule => BuildFilterExpressionTree(parameter, rule))
                    .Where(exp => exp != null)
                    .Aggregate((prev, next) => filter.Condition == ConditionType.OrElse ?
                                               Expression.OrElse(prev, next) : Expression.AndAlso(prev, next));
            }
            else if (!string.IsNullOrEmpty(filter.Field?.Trim()))
            {
                expression = BuildOperatorExpression(parameter, filter);
            }

            return filter.Not ? Expression.Not(expression) : expression;
        }

        private static Expression BuildOperatorExpression(Expression parameter, FilterRule filter)
        {
            var property = filter
                    .Field
                    .Split('.')
                    .Aggregate(parameter, Expression.Property);

            var value = filter.Operator == FilterOperator.Between || filter.Operator == FilterOperator.In ?
                null :
                Expression.Constant(ConvertOrDefaultValue(property.Type, filter.Value));

            return filter.Operator switch
            {
                FilterOperator.Eq => Expression.Equal(property, value),
                FilterOperator.Neq => Expression.NotEqual(property, value),
                FilterOperator.Gt => Expression.GreaterThan(property, value),
                FilterOperator.Lt => Expression.LessThan(property, value),
                FilterOperator.Lte => Expression.LessThanOrEqual(property, value),
                FilterOperator.Gte => Expression.GreaterThanOrEqual(property, value),
                FilterOperator.Between => Between(parameter, filter),
                FilterOperator.In => In(parameter, filter),
                FilterOperator.Contains => Expression.Call(property, nameof(string.Contains), Type.EmptyTypes, value),
                FilterOperator.StartsWith => Expression.Call(property, nameof(string.StartsWith), Type.EmptyTypes, value),
                FilterOperator.EndsWith => Expression.Call(property, nameof(string.EndsWith), Type.EmptyTypes, value),
                FilterOperator.IsNull => Expression.Equal(property, Expression.Constant(null)),
                FilterOperator.IsNotNull => Expression.NotEqual(property, Expression.Constant(null)),
                _ => throw new InvalidOperationException($"Invalid operation: {filter.Operator}"),
            };
        }

        private static Expression In(Expression property, FilterRule rule)
        {
            return CastToArray(property.Type, rule.Value)
                .Select(value => (Expression)Expression.Equal(property, Expression.Constant(value)))
                .Aggregate(Expression.OrElse);
        }

        private static BinaryExpression Between(Expression property, FilterRule rule)
        {
            var array = CastToArray(property.Type, rule.Value);

            if (array.Length < 2)
            {
                throw new IndexOutOfRangeException($"The array length less than 2.");
            }

            Array.Sort(array);

            var min = Expression.Constant(array[0]);
            var max = Expression.Constant(array[1]);

            var leftExpression = Expression.GreaterThanOrEqual(property, min);
            var rightExpression = Expression.LessThanOrEqual(property, max);

            return Expression.AndAlso(leftExpression, rightExpression);
        }

        private static object ConvertOrDefaultValue(Type propertyType, object value)
        {
            if (value is null)
            {
                if (propertyType.IsNullable())
                {
                    return value;
                }

                throw new InvalidCastException($"Can not convert [null] to type {propertyType}");
            }

            var valueType = value.GetType();

            if (propertyType != valueType)
            {
                var convertor = ComponentModel.TypeDescriptor.GetConverter(propertyType);
                var castedValue = (valueType == typeof(string)) ?
                    convertor.ConvertFromString(value.ToString()) :
                    convertor.ConvertFrom(value);

                return castedValue;
            }

            return value;
        }

        private static object[] CastToArray(Type targetType, object value)
        {
            if (value is not Array array)
            {
                throw new InvalidCastException($"value [{value}] is not array instance.");
            }

            var list = new List<object>();

            foreach (var item in array)
            {
                list.Add(ConvertOrDefaultValue(targetType, item));
            }

            return [.. list];
        }

        private static bool IsNullable(this Type type)
        {
            return !type.IsValueType && Nullable.GetUnderlyingType(type) != null;
        }
    }
}
