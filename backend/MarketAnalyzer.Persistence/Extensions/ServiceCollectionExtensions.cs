using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Persistence;
using MarketAnalyzer.Persistence.Extensions;

namespace Microsoft.Extensions.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFundPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEntityFrameworkSqlServer();
            services.AddSingleton<IMethodCallTranslatorPlugin, SqlServerMethodCallTranslatorPlugin>();
            services.AddDbContext<FundDbContext>((provider, options) =>
            {
                options.EnableSensitiveDataLogging(true);
                options.UseSqlServer(configuration.GetConnectionString("Defualt"), builder =>
                {
                    builder.CommandTimeout(5 * 60);
                    builder.MigrationsAssembly(typeof(FundDbContext).Assembly.FullName);
                    builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });

                options.UseInternalServiceProvider(provider);
                options.UseSqlServerMethodTranslator(provider);
            });

            services.TryAddScoped<IDbContext>(p => p.GetRequiredService<FundDbContext>());
            services.AddPersistenceApplicationPart(typeof(EnumerationsDataSeed).Assembly, 0);

            return services;
        }
    }
}
