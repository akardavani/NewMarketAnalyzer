using MarketAnalyzer.Domain;

namespace MarketAnalyzer.AnalysisStrategy
{
    public interface ICalculateImeContractIndex 
    {
        Task<List<MarketCandle>> CreateNearestDueDateContractIndex(string symbolStart, DateTime startDate, DateTime endDate, CancellationToken cancellation);
        Task<List<MarketCandle>> CreateSecondDueDateContractIndex(string symbolStart, DateTime startDate, DateTime endDate, CancellationToken cancellation);

        Task<List<MarketCandle>> CreateLatestDueDateContractIndex(string symbolStart, DateTime startDate, DateTime endDate, CancellationToken cancellation);
    }
}
