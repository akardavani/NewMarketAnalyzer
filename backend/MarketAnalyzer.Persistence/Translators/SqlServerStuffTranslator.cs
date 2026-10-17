using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MarketAnalyzer.Persistence.Translators
{
    internal class SqlServerStuffTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _method = typeof(SqlDbFunctions)
            .GetRuntimeMethod(nameof(SqlDbFunctions.Stuff), new[] { typeof(DbFunctions), typeof(string), typeof(int), typeof(int), typeof(string) });

        public SqlServerStuffTranslator()
        {

        }

        public SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (method != _method)
            {
                return null;
            }

            var characterExpression = arguments[1];
            var start = arguments[2];
            var length = arguments[3];
            var replaceWithExpression = arguments[4];

            return new SqlFunctionExpression(
                functionName: "STUFF",
                arguments: new[] { characterExpression, start, length, replaceWithExpression },
                nullable: true,
                argumentsPropagateNullability: new[] { false, true },
                type: method.ReturnType,
                typeMapping: null);
        }
    }
}
