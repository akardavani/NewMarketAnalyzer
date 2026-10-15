using MarketAnalyzer.Domain;
using System.Linq.Expressions;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    public interface IInvestmentFundService : IInfrastructureService
    {
        /// <summary>
        /// گرفتن دیتای صندوق ها 
        /// </summary>
        Task<List<InvestmentFund>> GetInvestmentFundAsync(Expression<Func<InvestmentFund, bool>>? filter, CancellationToken cancellation);
        /// <summary>
        /// گرفتن داینامیک کندل ها
        /// </summary>
        Task<List<MarketCandle>> GetCandlesAsync(Expression<Func<InvestmentFundChart, bool>>? filter, CancellationToken cancellation);
        /// <summary>
        /// ذخیره نوع صندوق
        /// </summary>
        Task SaveInvestmentFundTypeAsync(List<InvestmentFundType> data, CancellationToken cancellation);
        /// <summary>
        /// ذخیره دیتای صندوق ها
        /// </summary>
        Task SaveInvestmentFundAsync(List<InvestmentFund> data, CancellationToken cancellation);        
        /// <summary>
        /// قیمت صدور و ابطال صندوق
        /// </summary>
        Task SaveInvestmentFundChartAsync(List<InvestmentFundChart> data, CancellationToken cancellation);
        /// <summary>
        /// Nav محاسبه گپ
        /// </summary>
        Task CalculateFundNavGap(CancellationToken cancellation);
        Task DeleteDuplicateDataAsync(CancellationToken cancellation);
    }
}
