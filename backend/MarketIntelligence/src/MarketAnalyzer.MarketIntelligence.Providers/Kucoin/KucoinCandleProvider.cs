using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>کندل روزانهٔ بازار کریپتو (دلاری) از KuCoin. روز بر اساس UTC است.</summary>
    public sealed class KucoinCandleProvider : IHistoricalCandleProvider
    {
        private readonly KucoinClient _client;

        /// <summary>سازنده.</summary>
        public KucoinCandleProvider(KucoinClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public ProviderDescriptor Descriptor { get; } = Markets.Crypto(KucoinClient.ProviderKey, isDefault: true);

        /// <inheritdoc />
        public Task<FetchResult<CandleBatch>> FetchCandlesAsync(CandleRequest request, CancellationToken cancellationToken)
            => _client.FetchCandlesAsync(request, cancellationToken);
    }
}
