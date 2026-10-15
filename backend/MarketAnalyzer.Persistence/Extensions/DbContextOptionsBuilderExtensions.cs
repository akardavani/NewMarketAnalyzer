using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MarketAnalyzer.Persistence.Extensions;
using System;

namespace Microsoft.Extensions.Configuration
{
    internal static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseSqlServerMethodTranslator(this DbContextOptionsBuilder optionsBuilder, IServiceProvider serviceProvider)
        {
            return optionsBuilder
                .WithOption<SqlServerMethodTranslatorExtension>(extension => extension.WithSqlServerMethodTranslator(serviceProvider));
        }

        private static DbContextOptionsBuilder WithOption<TExtension>(
            this DbContextOptionsBuilder optionsBuilder,
            Func<TExtension, TExtension> withFunc)
            where TExtension : class, IDbContextOptionsExtension, new()
        {
            ArgumentNullException.ThrowIfNull(optionsBuilder);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(
                withFunc(optionsBuilder.Options.FindExtension<TExtension>() ?? new TExtension()));

            return optionsBuilder;
        }
    }
}
