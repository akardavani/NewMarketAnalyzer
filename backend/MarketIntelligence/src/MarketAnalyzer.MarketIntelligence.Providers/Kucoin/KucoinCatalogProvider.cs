using System.Text.Json;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// فهرست جفت‌ارزهای KuCoin. کلید استاندارد همان <c>BASE-QUOTE</c> خود KuCoin است (مثل BTC-USDT).
    /// <b>IsActive یعنی عضو دنیای پیگیری‌شده:</b> جفت‌ارز در حال معامله باشد و Quote آن در تنظیم <c>QuoteCurrencies</c> (پیش‌فرض USDT) باشد.
    /// بقیه ذخیره می‌شوند ولی غیرفعال‌اند. کد قدیمی همهٔ جفت‌ارزها را (حدود هزار تا) کندل می‌گرفت.
    /// </summary>
    public sealed class KucoinCatalogProvider : ISymbolCatalogProvider
    {
        private readonly KucoinClient _client;
        private readonly ILogger<KucoinCatalogProvider> _logger;

        /// <summary>سازنده.</summary>
        public KucoinCatalogProvider(KucoinClient client, ILogger<KucoinCatalogProvider> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <inheritdoc />
        public ProviderDescriptor Descriptor { get; } = Markets.Crypto(KucoinClient.ProviderKey, isDefault: true);

        /// <inheritdoc />
        public async Task<FetchResult<IReadOnlyList<SymbolInfo>>> FetchSymbolsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var quotes = await _client.GetQuoteCurrenciesAsync(cancellationToken);
                var rows = await _client.FetchSymbolsAsync(cancellationToken);
                var marketCurrency = Descriptor.CurrencyCode;

                var list = new Dictionary<string, SymbolInfo>(StringComparer.OrdinalIgnoreCase);
                foreach (var row in rows)
                {
                    var key = Str(row, "symbol");
                    var baseCurrency = Str(row, "baseCurrency");
                    var quote = Str(row, "quoteCurrency");
                    if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(quote))
                        continue;

                    var trading = row.TryGetProperty("trading", out var t) && t.ValueKind == JsonValueKind.True;

                    list[key] = new SymbolInfo
                    {
                        SymbolKey = key,
                        Name = Str(row, "symbolCode") ?? key,
                        IsActive = trading && quotes.Contains(quote),
                        CurrencyCode = string.Equals(quote, marketCurrency, StringComparison.OrdinalIgnoreCase) ? null : quote,
                        MetadataJson = JsonSerializer.Serialize(new { baseCurrency, quoteCurrency = quote, market = Str(row, "market") }),
                        MetadataVersion = 1
                    };
                }

                return list.Count == 0
                    ? FetchResult<IReadOnlyList<SymbolInfo>>.SuccessEmpty()
                    : FetchResult<IReadOnlyList<SymbolInfo>>.Success(list.Values.ToList(), list.Count);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "KuCoin symbol fetch failed");
                return FetchResult<IReadOnlyList<SymbolInfo>>.Failed($"KuCoin symbol fetch failed: {ex.Message}", ex);
            }
        }

        private static string? Str(JsonElement row, string name)
            => row.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
    }
}
