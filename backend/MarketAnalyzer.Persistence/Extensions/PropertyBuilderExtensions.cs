using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketAnalyzer.Persistence.ValueConversions;

namespace MarketAnalyzer.Persistence.Extensions
{
    internal static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<T> HasJsonValueConversion<T>(this PropertyBuilder<T> propertyBuilder) where T : class
        {
            propertyBuilder
              .HasConversion(new JsonValueConverter<T>())
              .Metadata
              .SetValueComparer(new JsonValueComparer<T>());

            return propertyBuilder;
        }
    }
}
