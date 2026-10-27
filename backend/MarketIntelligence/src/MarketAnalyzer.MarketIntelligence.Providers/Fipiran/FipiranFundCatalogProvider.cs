using System.Text.Json;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// فهرست صندوق‌های سرمایه‌گذاری از فیپیران. کلید نماد شمارهٔ ثبت (regNo) است. اگر منبع کمتر از «تعداد کل» اعلام‌شده بدهد
    /// (صفحه‌بندی)، نتیجه <c>Partial</c> است و هیچ صندوقی غیرفعال نمی‌شود.
    /// </summary>
    public sealed class FipiranFundCatalogProvider : ISymbolCatalogProvider
    {
        private readonly FipiranClient _client;
        private readonly ILogger<FipiranFundCatalogProvider> _logger;

        /// <summary>سازنده.</summary>
        public FipiranFundCatalogProvider(FipiranClient client, ILogger<FipiranFundCatalogProvider> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <inheritdoc />
        public ProviderDescriptor Descriptor { get; } = Markets.Fund(FipiranClient.ProviderKey, isDefault: true);

        /// <inheritdoc />
        public async Task<FetchResult<IReadOnlyList<SymbolInfo>>> FetchSymbolsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var (items, total) = await _client.GetFundsAsync(cancellationToken);

                var list = new Dictionary<string, SymbolInfo>(StringComparer.Ordinal);
                foreach (var item in items)
                {
                    var regNo = Text(item, "regNo");
                    if (string.IsNullOrWhiteSpace(regNo))
                        continue;

                    var name = Text(item, "name");
                    list[regNo] = new SymbolInfo
                    {
                        SymbolKey = regNo,
                        ExternalId = regNo,
                        Name = name,
                        PersianName = name,
                        MetadataJson = JsonSerializer.Serialize(new
                        {
                            fundType = Int(item, "fundType"),
                            typeOfInvest = Text(item, "typeOfInvest"),
                            manager = Text(item, "manager"),
                            initiationDate = Text(item, "initiationDate"),
                            isCompleted = item.TryGetProperty("isCompleted", out var c) && c.ValueKind == JsonValueKind.True
                        }),
                        MetadataVersion = 1
                    };
                }

                if (list.Count == 0)
                    return FetchResult<IReadOnlyList<SymbolInfo>>.SuccessEmpty();

                var data = (IReadOnlyList<SymbolInfo>)list.Values.ToList();
                return list.Count < total
                    ? FetchResult<IReadOnlyList<SymbolInfo>>.Partial(data, data.Count, $"Source reports {total} funds but returned {list.Count} (paged?).")
                    : FetchResult<IReadOnlyList<SymbolInfo>>.Success(data, data.Count);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or FormatException or InvalidOperationException)
            {
                if (ex is TaskCanceledException && cancellationToken.IsCancellationRequested)
                    throw;

                _logger.LogError(ex, "Fipiran fund list fetch failed");
                return FetchResult<IReadOnlyList<SymbolInfo>>.Failed($"Fipiran fund list failed: {ex.Message}", ex);
            }
        }

        private static string? Text(JsonElement e, string name)
            => e.TryGetProperty(name, out var v) ? v.ValueKind switch
            {
                JsonValueKind.String => v.GetString(),
                JsonValueKind.Number => v.GetRawText(),
                _ => null
            } : null;

        private static int? Int(JsonElement e, string name)
            => e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out var n) ? n : null;
    }
}
