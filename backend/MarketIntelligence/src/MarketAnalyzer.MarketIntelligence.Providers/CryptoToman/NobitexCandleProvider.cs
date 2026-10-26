using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// کندل روزانهٔ بازار کریپتو-تومان از Nobitex؛ جایگزین Wallex. برای فعال‌کردن، در MarketSettings (Scope = market:crypto-toman،
    /// Key = Provider:HistoricalCandles) مقدار nobitex را بگذار. بعد از تعویض منبع، کندل‌های این بازار پاک و از منبع جدید کامل دوباره دریافت می‌شوند.
    /// </summary>
    public sealed class NobitexCandleProvider : IHistoricalCandleProvider
    {
        private readonly NobitexCandleClient _client;

        /// <summary>سازنده.</summary>
        public NobitexCandleProvider(NobitexCandleClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public ProviderDescriptor Descriptor { get; } = Markets.CryptoToman(NobitexCandleClient.ProviderKey);

        /// <inheritdoc />
        public Task<FetchResult<CandleBatch>> FetchCandlesAsync(CandleRequest request, CancellationToken cancellationToken)
            => _client.FetchAsync(request, cancellationToken);
    }
}
