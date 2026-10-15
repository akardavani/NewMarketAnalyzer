using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using MarketAnalyzer.Persistence.Translators;
using System.Collections.Generic;

namespace MarketAnalyzer.Persistence.Extensions
{
    internal class SqlServerMethodCallTranslatorPlugin : IMethodCallTranslatorPlugin
    {
        public SqlServerMethodCallTranslatorPlugin(IRelationalTypeMappingSource typeMappingSource)
        {
            Translators =
            [
                new SqlServerCastTranslator(),
                new SqlServerStuffTranslator(),
                new SqlServerSumOverTranslator(),
                new SqlServerCoalesceTranslator(),
                new SqlServerConvertTranslator(typeMappingSource),
                new SqlServerRowNumberTranslator( typeMappingSource),
            ];
        }

        public IEnumerable<IMethodCallTranslator> Translators { get; }
    }
}
