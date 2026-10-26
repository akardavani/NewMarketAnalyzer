using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>کندل روزانهٔ بازار کریپتو-تومان از Wallex. Provider پیش‌فرض این بازار است (همان که در کد قدیمی واقعاً کار می‌کرد).</summary>
    public sealed class WallexCandleProvider : IHistoricalCandleProvider
    {
        private readonly WallexCandleClient _client;

        /// <summary>سازنده.</summary>
        public WallexCandleProvider(WallexCandleClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public ProviderDescriptor Descriptor { get; } = Markets.CryptoToman(WallexCandleClient.ProviderKey, isDefault: true);

        /// <inheritdoc />
        public Task<FetchResult<CandleBatch>> FetchCandlesAsync(CandleRequest request, CancellationToken cancellationToken)
            => _client.FetchAsync(request, cancellationToken);
    }
}
