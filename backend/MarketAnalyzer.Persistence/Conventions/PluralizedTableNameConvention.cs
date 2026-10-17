using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MarketAnalyzer.Abstractions;
using System;
using System.Linq;

namespace MarketAnalyzer.Persistence.Conventions
{
    internal class PluralizedTableNameConvention : IEntityTypeAddedConvention
    {
        public void ProcessEntityTypeAdded(IConventionEntityTypeBuilder entityTypeBuilder, IConventionContext<IConventionEntityTypeBuilder> context)
        {
            if (entityTypeBuilder.Metadata.IsOwned() || entityTypeBuilder.Metadata.ClrType.IsDefined(typeof(OwnedEntityAttribute), true))
            {
                return;
            }

            var tableName = entityTypeBuilder
                 .Metadata
                 .GetTableName()
                 .Pluralize(inputIsKnownToBeSingular: false);

            entityTypeBuilder.Metadata.SetTableName(tableName);

            var clrType = entityTypeBuilder.Metadata.ClrType;
            const string rootNamespace = "MarketAnalyzer.";

            if (clrType is not null &&
                clrType.Namespace is not null &&
                clrType.Namespace.StartsWith(
                    rootNamespace,
                    StringComparison.Ordinal))
            {
                var schema = clrType.Namespace[rootNamespace.Length..]
                    .Split('.')
                    .FirstOrDefault();

                if (!string.IsNullOrEmpty(schema))
                {
                    entityTypeBuilder.Metadata.SetSchema(schema.ToLowerInvariant());
                }
            }
        }
    }
}
