using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq
{
    public static class ExpressionHelper
    {
        public static Expression<Func<TSource, bool>> BuildSingleExpression<TSource, TKey>(
            this IEnumerable<TKey> values,
            IQueryable<TSource> _,
            Expression<Func<TSource, TKey>> keySelector)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (keySelector is null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            var parameter = keySelector.Parameters.Single();
            var equals = values.Select(value => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(value, typeof(TKey))));
            var body = equals.Aggregate(Expression.OrElse);
            return Expression.Lambda<Func<TSource, bool>>(body, parameter);
        }

        public static Expression<Func<TSource, bool>> Merge<TSource>(
            this Expression<Func<TSource, bool>> left,
            Expression<Func<TSource, bool>> right,
            ConditionType conditionType)
        {
            var visitor = new ReplaceParameterVisitor(right.Parameters[0], left.Parameters[0]);
            var body = visitor.Visit(right.Body);
            var merged = conditionType == ConditionType.AndAlso ?
                Expression.AndAlso(left.Body, body) :
                Expression.OrElse(left.Body, body);
            return Expression.Lambda<Func<TSource, bool>>(merged, left.Parameters[0]);
        }

        public static Expression<Func<TSource, bool>> Merge<TSource>(this IEnumerable<Expression<Func<TSource, bool>>> source, ConditionType conditionType)
        {
            if (!source.Any())
            {
                return e => true;
            }
            return source.Aggregate((left, right) => Merge(left, right, conditionType));
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter;
            private readonly ParameterExpression _newParameter;

            public ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {

                return ReferenceEquals(node, _oldParameter) ?
                    _newParameter :
                    base.VisitParameter(node);
            }
        }
    }
}
