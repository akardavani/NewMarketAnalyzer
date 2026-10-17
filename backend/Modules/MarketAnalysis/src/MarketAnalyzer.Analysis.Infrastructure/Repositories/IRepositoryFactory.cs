using MarketAnalyzer.Domain;
namespace MarketAnalyzer.Analysis.Infrastructure
{
    public interface IRepositoryFactory
    {
        IRepository<T> Get<T>() where T : class, IEntity;
    }

}


