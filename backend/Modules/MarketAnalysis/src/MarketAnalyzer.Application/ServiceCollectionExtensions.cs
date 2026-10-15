using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketAnalyzer.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMarketAnalyzerApplication(this IServiceCollection services)
        {
            //services.AddCqrs(typeof(ServiceCollectionExtensions).Assembly);
            return services;
        }
    }
}
