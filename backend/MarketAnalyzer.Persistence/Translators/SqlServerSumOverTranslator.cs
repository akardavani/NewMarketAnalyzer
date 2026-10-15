using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace MarketAnalyzer.Persistence.Translators
{
    internal class SqlServerSumOverTranslator : IMethodCallTranslator
    {
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public SqlServerSumOverTranslator()
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

            if (method.Name == nameof(SqlDbFunctions.SumOver) && method.DeclaringType == typeof(SqlDbFunctions))
            {
                //_sqlExpressionFactory.("",)

                var ccc = new SqlFunctionExpression(
                    functionName: "SUM",
                    arguments: arguments.Skip(1),
                    nullable: true,
                    argumentsPropagateNullability: [true],
                    type: typeof(decimal),
                    typeMapping: null);

                var test = ccc.Print();

                return new SqlFragmentExpression(arguments[1].Print());
                //TODO: Create SqlExpression with merge SqlFunctionExpression and OrderByExpression
                return new SqlFunctionExpression(
                    functionName: "SUM",
                    arguments: arguments,
                    nullable: true,
                    argumentsPropagateNullability: [true],
                    type: typeof(decimal),
                    typeMapping: null);
            }

            return null;
        }
    }
}
