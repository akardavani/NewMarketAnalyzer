using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketAnalyzer.Persistence
{
    internal static class OwnedNavigationBuilderExtensions
    {
        public static OwnedNavigationBuilder<TOwnerEntity, TDependentEntity> ConfigureByInt64Key<TOwnerEntity, TDependentEntity>(
            this OwnedNavigationBuilder<TOwnerEntity, TDependentEntity> builder,
            string primaryKeyName = "Id")
            where TOwnerEntity : class
            where TDependentEntity : class
        {
            ConfigureByKey<long, TOwnerEntity, TDependentEntity>(builder, primaryKeyName);
            return builder;
        }

        public static OwnedNavigationBuilder<TOwnerEntity, TDependentEntity> ConfigureByInt32Key<TOwnerEntity, TDependentEntity>(
            this OwnedNavigationBuilder<TOwnerEntity, TDependentEntity> builder,
            string primaryKeyName = "Id")
            where TOwnerEntity : class
            where TDependentEntity : class
        {
            ConfigureByKey<int, TOwnerEntity, TDependentEntity>(builder, primaryKeyName);
            return builder;
        }

        private static OwnedNavigationBuilder<TOwnerEntity, TDependentEntity> ConfigureByKey<TKey, TOwnerEntity, TDependentEntity>(
            this OwnedNavigationBuilder<TOwnerEntity, TDependentEntity> builder,
            string primaryKeyName = "Id")
            where TOwnerEntity : class
            where TDependentEntity : class
        {
            builder.Property<TKey>(primaryKeyName)
                .HasColumnOrder(0);
            builder.HasKey(primaryKeyName);

            var fk = $"{typeof(TOwnerEntity).Name}{primaryKeyName}";
            builder.Property(fk)
                .HasColumnOrder(1);

            builder.WithOwner()
                .HasForeignKey(fk);

            return builder;
        }
    }
}
