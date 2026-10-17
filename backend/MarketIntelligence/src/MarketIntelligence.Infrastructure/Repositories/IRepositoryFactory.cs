using MarketIntelligence.Domain;
namespace MarketIntelligence.Infrastructure
{
    public interface IRepositoryFactory
    {
        IRepository<T> Get<T>() where T : class, IEntity;
    }

}


