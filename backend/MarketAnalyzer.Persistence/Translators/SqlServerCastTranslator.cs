using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace MarketAnalyzer.Persistence.Translators
{
    internal class SqlServerCastTranslator : IMethodCallTranslator
    {
        private readonly Dictionary<MethodInfo, string> _methodInfoConvertMapping = new()
        {
            { typeof(SqlDbFunctions).GetRuntimeMethod(nameof(SqlDbFunctions.Cast), [typeof(DbFunctions), typeof(decimal?), typeof(int), typeof(int)]), "DECIMAL" },
        };

        public SqlServerCastTranslator()
        {

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

            if (_methodInfoConvertMapping.TryGetValue(method, out var targetType))
            {
                var expression = (ColumnExpression)arguments[1];
                var precision = (SqlConstantExpression)arguments[2];
                var scale = (SqlConstantExpression)arguments[3];

                return new SqlFunctionExpression(
                    functionName: "CAST",
                    arguments: [new SqlFragmentExpression($"[{expression.TableAlias}].[{expression.Name}] AS {targetType}({precision.Value}, {scale.Value})")],
                    nullable: true,
                    argumentsPropagateNullability: [true],
                    type: method.ReturnType,
                    typeMapping: null);
            }

            return null;
        }
    }
}
