using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.DependencyInjection;
namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public RepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IRepository<T> Get<T>() where T : class, MarketAnalyzer.MarketIntelligence.Domain.IEntity
        {
            return _serviceProvider.GetRequiredService<IRepository<T>>();
        }
    }

}


