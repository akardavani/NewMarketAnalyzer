using MarketAnalyzer.MarketData.Domain;
using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application
{
    public interface ICalculateImeContractIndex 
    {
        Task<List<AnalysisCandle>> CreateNearestDueDateContractIndex(string symbolStart, DateTime startDate, DateTime endDate, CancellationToken cancellation);
        Task<List<AnalysisCandle>> CreateSecondDueDateContractIndex(string symbolStart, DateTime startDate, DateTime endDate, CancellationToken cancellation);

        Task<List<AnalysisCandle>> CreateLatestDueDateContractIndex(string symbolStart, DateTime startDate, DateTime endDate, CancellationToken cancellation);
    }
}
