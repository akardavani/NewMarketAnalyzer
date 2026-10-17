using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.Messaging
{
    internal class MessagingHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MessagingHostedService> _logger;
        private readonly IEnumerable<IMessageServiceProviderFactory> _serviceProviderFactories;

        public MessagingHostedService(
            IServiceProvider serviceProvider,
            ILogger<MessagingHostedService> logger,
            IEnumerable<IMessageServiceProviderFactory> serviceProviderFactories)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _serviceProviderFactories = serviceProviderFactories;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
            var providers = await dbContext
                .Set<MessageServiceProviderConfig>()
                .AsNoTracking()
                .Where(e => e.IsActive)
                .ToListAsync(stoppingToken);

            foreach (var provider in providers)
            {
                foreach (var providerFactory in _serviceProviderFactories)
                {
                    _logger.LogWarning("ProviderFactory: " + providerFactory.GetType().Name +
                                       " Is Starting For Provider " + provider.Name + "...");
                    if (providerFactory.IsSupported(provider))
                    {
                        _logger.LogWarning("ProviderFactory: " + providerFactory.GetType().Name +
                                           " Creating For Provider " + provider.Name + "...");
                        var messageService = providerFactory.Create(provider, stoppingToken);
                        _logger.LogWarning("ProviderFactory: " + providerFactory.GetType().Name +
                                           " Created For Provider " + provider.Name + "...");
                        await messageService.StartAsync(stoppingToken);
                        _logger.LogWarning("ProviderFactory: " + providerFactory.GetType().Name +
                                           " Started For Provider " + provider.Name + "...");
                    }
                    else
                    {
                        _logger.LogWarning("ProviderFactory: " + providerFactory.GetType().Name +
                                           " NotSupported For Provider " + provider.Name + "...");

                    }
                }
            }
        }
    }
}
