using MarketAnalyzer.Domain;
using System.Linq.Expressions;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    public interface IStockService : IInfrastructureService//,IInfrastructureService
    {
        /// <summary>
        /// ذخیره آستانه قیمتی 
        /// </summary>
        Task SaveTseStaticThresholdAsync(List<TseStaticThreshold> data, CancellationToken cancellation);
        /// <summary>
        /// ذخیره قیمت های سرخطی 
        /// </summary>
        Task SaveTseBestLimitAsync(List<TseBestLimit> data, CancellationToken cancellation);

        
        Task SaveTseTradeLastDayAsync(List<TseTradeLastDay> data, CancellationToken cancellation);
    }
}

