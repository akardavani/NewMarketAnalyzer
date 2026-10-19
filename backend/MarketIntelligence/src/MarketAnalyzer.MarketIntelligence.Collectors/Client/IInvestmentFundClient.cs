using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Collectors
{
    public interface IInvestmentFundClient
    {
        Task<List<InvestmentFund>> GetInvestmentFund(CancellationToken cancellation);
        Task<List<InvestmentFundType>> GetFundType(CancellationToken cancellation);
        Task<InvestmentFundInfo> GetInvestmentFundInfo(string regNo, CancellationToken cancellation);
        /// <summary>
        /// قیمت صدور و ابطال صندوق
        /// </summary>
        Task<List<InvestmentFundChart>> GetInvestmentFundChart(string regNo, CancellationToken cancellation);
    }
}
