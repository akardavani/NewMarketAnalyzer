using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>قیمت ماهانهٔ مسکن هر منطقه از کیلید (داده ماهانهٔ بومی).</summary>
    public sealed class KilidCandleProvider : IHistoricalCandleProvider
    {
        private readonly KilidCandleClient _client;

        /// <summary>سازنده.</summary>
        public KilidCandleProvider(KilidCandleClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public ProviderDescriptor Descriptor { get; } = Markets.RealEstate(KilidCandleClient.ProviderKey, isDefault: true, maxRequestsPerMinute: 30);

        /// <inheritdoc />
        public Task<FetchResult<CandleBatch>> FetchCandlesAsync(CandleRequest request, CancellationToken cancellationToken)
            => _client.FetchAsync(request, cancellationToken);
    }
}
