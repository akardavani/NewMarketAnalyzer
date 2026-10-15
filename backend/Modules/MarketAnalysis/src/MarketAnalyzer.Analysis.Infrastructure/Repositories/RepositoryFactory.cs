using MarketAnalyzer.Domain;
using Microsoft.Extensions.DependencyInjection;
namespace MarketAnalyzer.Analysis.Infrastructure
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public RepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IRepository<T> Get<T>() where T : class, IEntity
        {
            return _serviceProvider.GetRequiredService<IRepository<T>>();
        }
    }

}


