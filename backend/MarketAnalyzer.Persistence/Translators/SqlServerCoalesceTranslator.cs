using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MarketAnalyzer.Persistence.Translators
{
    internal class SqlServerCoalesceTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _coalesceMethod = typeof(SqlDbFunctions).GetMethod(nameof(SqlDbFunctions.Coalesce));

        public SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (method != _coalesceMethod)
            {
                return null;
            }

            throw new NotImplementedException();
        }
    }
}
