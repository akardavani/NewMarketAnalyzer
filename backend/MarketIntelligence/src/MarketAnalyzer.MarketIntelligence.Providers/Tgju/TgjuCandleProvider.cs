using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>کندل روزانهٔ بازار کلان از Tgju.</summary>
    public sealed class TgjuCandleProvider : IHistoricalCandleProvider
    {
        private readonly TgjuCandleClient _client;

        /// <summary>سازنده.</summary>
        public TgjuCandleProvider(TgjuCandleClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public ProviderDescriptor Descriptor { get; } = Markets.Macro(TgjuCandleClient.ProviderKey);

        /// <inheritdoc />
        public Task<FetchResult<CandleBatch>> FetchCandlesAsync(CandleRequest request, CancellationToken cancellationToken)
            => _client.FetchAsync(request, cancellationToken);
    }
}
