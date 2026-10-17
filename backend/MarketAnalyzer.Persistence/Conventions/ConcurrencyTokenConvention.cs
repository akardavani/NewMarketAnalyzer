using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MarketAnalyzer.Abstractions;
using System.Reflection;

namespace MarketAnalyzer.Persistence.Conventions
{
    internal class ConcurrencyTokenConvention : IPropertyAddedConvention
    {
        public void ProcessPropertyAdded(IConventionPropertyBuilder propertyBuilder, IConventionContext<IConventionPropertyBuilder> context)
        {
            var memberInfo = GetIdentifyingMemberInfo(propertyBuilder.Metadata);
            if (memberInfo == null)
            {
                return;
            }

            if (typeof(IConcurrencyEntity).IsAssignableFrom(memberInfo.DeclaringType) && memberInfo.Name == nameof(IConcurrencyEntity.RowVersion))
            {
                propertyBuilder.HasMaxLength(8, false);
                propertyBuilder.ValueGenerated(ValueGenerated.OnAddOrUpdate, fromDataAnnotation: false);
                propertyBuilder.IsConcurrencyToken(true, false);
            }
        }

        private static MemberInfo GetIdentifyingMemberInfo(IReadOnlyPropertyBase propertyBase)
        {
            var indexerPropertyInfo = propertyBase.DeclaringType.FindIndexerPropertyInfo();
            return indexerPropertyInfo != null && propertyBase.PropertyInfo == indexerPropertyInfo
                ? null
                : (propertyBase.PropertyInfo ?? (MemberInfo)propertyBase.FieldInfo);
        }
    }
}
