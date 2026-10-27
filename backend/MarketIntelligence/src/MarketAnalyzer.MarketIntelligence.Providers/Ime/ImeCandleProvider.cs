using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>کندل روزانهٔ قراردادهای آتی بورس کالا.</summary>
    public sealed class ImeCandleProvider : IHistoricalCandleProvider
    {
        private readonly ImeCandleClient _client;

        /// <summary>سازنده.</summary>
        public ImeCandleProvider(ImeCandleClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public ProviderDescriptor Descriptor { get; } = Markets.Ime(ImeCandleClient.ProviderKey, isDefault: true, maxRequestsPerMinute: 120);

        /// <inheritdoc />
        public Task<FetchResult<CandleBatch>> FetchCandlesAsync(CandleRequest request, CancellationToken cancellationToken)
            => _client.FetchAsync(request, cancellationToken);
    }
}
