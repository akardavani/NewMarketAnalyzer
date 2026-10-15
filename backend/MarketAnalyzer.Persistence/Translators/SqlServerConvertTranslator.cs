using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MarketAnalyzer.Persistence.Translators
{
    internal class SqlServerConvertTranslator : IMethodCallTranslator
    {
        private readonly Dictionary<MethodInfo, string> _methodInfoConvertMapping = new()
        {
            { typeof(SqlDbFunctions).GetRuntimeMethod(nameof(SqlDbFunctions.Convert), [typeof(DbFunctions), typeof(DateTime)]), "DATE" },
            { typeof(SqlDbFunctions).GetRuntimeMethod(nameof(SqlDbFunctions.Convert), [typeof(DbFunctions), typeof(DateTime?)]), "DATE" },
        };
        private static readonly DateOnlyTypeMapping _date = new();
        private readonly IRelationalTypeMappingSource _typeMappingSource;

        public SqlServerConvertTranslator(IRelationalTypeMappingSource typeMappingSource)
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

            if (_methodInfoConvertMapping.TryGetValue(method, out var targetType))
            {
                RelationalTypeMapping typeMapping;
                if (method.ReturnType == typeof(DateOnly))
                {
                    typeMapping = _date;
                }
                else
                {
                    typeMapping = _typeMappingSource.FindMapping(method.ReturnType);
                }

                var input = arguments[1];
                return new SqlFunctionExpression(
                    functionName: "CONVERT",
                    arguments: [new SqlFragmentExpression(targetType), input],
                    nullable: true,
                    argumentsPropagateNullability: [false, true],
                    type: method.ReturnType,
                    typeMapping: typeMapping);
            }

            return null;
        }

        private class DateOnlyTypeMapping : RelationalTypeMapping
        {
            public const string DateFormatConst = "'{0:yyyy-MM-dd}'";

            public DateOnlyTypeMapping()
                : base(new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(typeof(DateTime)),
                    "date",
                    StoreTypePostfix.Precision,
                    System.Data.DbType.Date))
            {
            }

            protected override string SqlLiteralFormatString => DateFormatConst;

            public override ValueConverter Converter => null;

            protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => this;
        }
    }
}
