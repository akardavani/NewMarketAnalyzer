using System.Collections.Concurrent;
using System.Globalization;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// خرید و فروش حقیقی/حقوقی روزانهٔ نمادهای فعال بورس. به‌ازای هر نماد یک درخواست HTTP می‌زند، پس با همزمانی محدود
    /// (<see cref="MaxConcurrency"/>) اجرا می‌شود. اگر بعضی نمادها شکست بخورند نتیجه Partial است (داده‌های سالم ذخیره می‌شوند،
    /// هیچ حذفی انجام نمی‌شود)؛ اگر همه شکست بخورند Failed.
    /// فقط <c>InvestorActivityHistoryDays</c> روز اخیر نگه داشته می‌شود (۰ = همهٔ تاریخچه، برای بار اول).
    /// </summary>
    public sealed class TsetmcStockInvestorActivityProvider : IInvestorActivityProvider
    {
        /// <summary>سقف درخواست همزمان به TSETMC.</summary>
        private const int MaxConcurrency = 4;

        private readonly TsetmcClientTypeClient _client;
        private readonly TseSymbolDirectory _directory;
        private readonly IProviderSettings _settings;
        private readonly TimeProvider _time;
        private readonly ILogger<TsetmcStockInvestorActivityProvider> _logger;

        /// <summary>سازنده.</summary>
        public TsetmcStockInvestorActivityProvider(
            TsetmcClientTypeClient client, TseSymbolDirectory directory, IProviderSettings settings,
            TimeProvider time, ILogger<TsetmcStockInvestorActivityProvider> logger)
        {
            _client = client;
            _directory = directory;
            _settings = settings;
            _time = time;
            _logger = logger;
        }

        /// <inheritdoc />
        public ProviderDescriptor Descriptor { get; } = TseMarket.Descriptor(TseMarket.TsetmcProviderKey);

        /// <inheritdoc />
        public async Task<FetchResult<IReadOnlyList<InvestorActivityInfo>>> FetchInvestorActivitiesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var symbols = await _directory.GetActiveAsync(cancellationToken);
                if (symbols.Count == 0)
                    return FetchResult<IReadOnlyList<InvestorActivityInfo>>.Failed("Stock catalog is empty; sync symbols before investor-activity ingestion.");

                var template = await _client.GetUrlTemplateAsync(cancellationToken);
                var days = int.TryParse(
                    await _settings.GetAsync(TseMarket.TsetmcProviderKey, TsetmcSettingDefaults.InvestorActivityDaysKey, cancellationToken),
                    NumberStyles.Integer, CultureInfo.InvariantCulture, out var d) ? d : 60;

                DateOnly? from = days > 0
                    ? DateOnly.FromDateTime(_time.GetUtcNow().UtcDateTime).AddDays(-days)
                    : null;

                var rows = new ConcurrentBag<InvestorActivityInfo>();
                var failed = new ConcurrentBag<string>();
                Exception? firstError = null;

                await Parallel.ForEachAsync(symbols,
                    new ParallelOptions { MaxDegreeOfParallelism = MaxConcurrency, CancellationToken = cancellationToken },
                    async (symbol, ct) =>
                    {
                        try
                        {
                            foreach (var item in await _client.FetchAsync(template, symbol, from, ct))
                                rows.Add(item);
                        }
                        catch (Exception ex) when (ex is not OperationCanceledException)
                        {
                            failed.Add(symbol.SymbolKey);
                            Interlocked.CompareExchange(ref firstError, ex, null);
                        }
                    });

                var list = rows.ToList();

                if (failed.Count == symbols.Count)
                    return FetchResult<IReadOnlyList<InvestorActivityInfo>>.Failed($"All {failed.Count} client-type requests failed: {firstError?.Message}", firstError);

                if (failed.Count > 0)
                {
                    _logger.LogWarning("Investor activity: {Failed}/{Total} symbols failed", failed.Count, symbols.Count);
                    return FetchResult<IReadOnlyList<InvestorActivityInfo>>.Partial(list, list.Count,
                        $"{failed.Count} of {symbols.Count} symbols failed. First error: {firstError?.Message}", firstError);
                }

                return list.Count == 0
                    ? FetchResult<IReadOnlyList<InvestorActivityInfo>>.SuccessEmpty()
                    : FetchResult<IReadOnlyList<InvestorActivityInfo>>.Success(list, list.Count);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "TSETMC investor activity fetch failed");
                return FetchResult<IReadOnlyList<InvestorActivityInfo>>.Failed($"TSETMC investor activity fetch failed: {ex.Message}", ex);
            }
        }
    }
}
