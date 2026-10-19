using MarketAnalyzer.MarketIntelligence.Domain;
namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public interface IRepositoryFactory
    {
        IRepository<T> Get<T>() where T : class, MarketAnalyzer.MarketIntelligence.Domain.IEntity;
    }

}


