using MarketAnalyzer.MarketIntelligence.Domain;
using MarketAnalyzer.MarketIntelligence.Infrastructure;

namespace MarketAnalyzer.MarketIntelligence.Collectors
{
    public class InvestmentFundCollector : BaseCollector
    {
        public override string CollectorName => GetType().Name;

        /// <summary>
        /// گرفتن دیتا از چند سال قبل
        /// </summary>
        int _yearsAgo = 1;
        /// <summary>
        /// نوع بازار
        /// </summary>
        MarketType _marketTypeDto = MarketType.InvestmentFund;

        private readonly IInvestmentFundClient _investmentFundClient;
        private readonly IInvestmentFundService _investmentFundService;
        private readonly MarketCollector _marketCollector;

        public InvestmentFundCollector(
            IInvestmentFundClient investmentFundClient,
             IInvestmentFundService investmentFundService,
            MarketCollector marketCollector)
        {
            _investmentFundClient = investmentFundClient;
            _investmentFundService = investmentFundService;
            _marketCollector = marketCollector;
        }

        public async Task FetchInvestmentFundType()
        {
            var fundType = await _investmentFundClient.GetFundType(cancellation: CancellationToken.None);

            await _investmentFundService.SaveInvestmentFundTypeAsync(fundType, cancellation: CancellationToken.None);
        }

        public async Task FetchInvestmentFund()
        {
            var investmentFund = await _investmentFundClient.GetInvestmentFund(cancellation: CancellationToken.None);
            await _investmentFundService.SaveInvestmentFundAsync(investmentFund, cancellation: CancellationToken.None);            
        }

        public async Task FetchInvestmentFundChart()
        {
            await DeleteDuplicateData(cancellation: CancellationToken.None);
            await CalculateFundNavGap(cancellation: CancellationToken.None);

            var list = new List<InvestmentFundChart>();

            var investmentFund = await _investmentFundService.GetInvestmentFundAsync(null, cancellation: CancellationToken.None);
            foreach (var item in investmentFund)
            {
                var investmentFundChart = await _investmentFundClient.GetInvestmentFundChart(item.RegNo, cancellation: CancellationToken.None);
                list.AddRange(investmentFundChart);
            }

            await _investmentFundService.SaveInvestmentFundChartAsync(list, cancellation: CancellationToken.None);
        }

        /// <summary>
        /// پیدا کردن گپ ها
        /// </summary>
        private async Task CalculateFundNavGap(CancellationToken cancellation)
        {
            await _investmentFundService.CalculateFundNavGap(cancellation);            
        }

        private async Task DeleteDuplicateData(CancellationToken cancellation)
        {
            await _investmentFundService.DeleteDuplicateDataAsync(cancellation);
        }

    }    
}
