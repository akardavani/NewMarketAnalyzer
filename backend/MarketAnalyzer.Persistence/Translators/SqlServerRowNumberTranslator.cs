using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace MarketAnalyzer.Persistence.Translators
{
    internal class SqlServerRowNumberTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _rowNumberMethod = typeof(SqlDbFunctions)
            .GetMethod(nameof(SqlDbFunctions.RowNumber), [typeof(DbFunctions), typeof(object), typeof(SortType)]);

        private readonly IRelationalTypeMappingSource _typeMappingSource;

        public SqlServerRowNumberTranslator(IRelationalTypeMappingSource typeMappingSource)
        {
            _typeMappingSource = typeMappingSource;
        }

        public SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            ArgumentNullException.ThrowIfNull(method);
            ArgumentNullException.ThrowIfNull(arguments);
            ArgumentNullException.ThrowIfNull(logger);

            if (method != _rowNumberMethod)
                return null;

            if (arguments[2] is not SqlConstantExpression { Value: SortType sortType })
                throw new ArgumentException("Invalid SortType expression", nameof(arguments));

            var orderExpr = arguments[1];
            if (orderExpr is SqlUnaryExpression { OperatorType: ExpressionType.Convert } u && u.Type == typeof(object))
            {
                orderExpr = u.Operand;
            }

            if (orderExpr.TypeMapping is null)
            {
                var clrType = Nullable.GetUnderlyingType(orderExpr.Type) ?? orderExpr.Type;
                var mapping = _typeMappingSource.FindMapping(clrType) ?? throw new InvalidOperationException($"No type mapping for {clrType}");

                orderExpr = new SqlUnaryExpression(ExpressionType.Convert, orderExpr, clrType, mapping);
            }

            var rowNumberMapping = _typeMappingSource.FindMapping(typeof(long)) ?? throw new InvalidOperationException("No mapping for bigint");

            return new RowNumberExpression(
                partitions: [],
                orderings: [new OrderingExpression(orderExpr, ascending: sortType == SortType.ASC)],
                typeMapping: rowNumberMapping);
        }
    }
}
