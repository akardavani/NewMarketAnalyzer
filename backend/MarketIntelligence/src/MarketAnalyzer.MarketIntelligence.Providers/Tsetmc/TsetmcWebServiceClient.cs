using System.Xml.Linq;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using MarketAnalyzer.MarketIntelligence.Providers.ConnectedServices.TsePublic;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// کلاینت وب‌سرویس SOAP سازمان بورس (TsePublicV2). فقط ردیف‌های خام XML را برمی‌گرداند.
    /// تفاوت با کد قدیمی: ۱) هر خطا استثنا می‌شود و به «لیست خالی» تبدیل نمی‌شود، ۲) اگر یکی از بازارها شکست بخورد
    /// کل دریافت شکست می‌خورد (دادهٔ ناقص به‌عنوان کامل ذخیره نشود)، ۳) channel هر بار بسته می‌شود.
    /// </summary>
    public sealed class TsetmcWebServiceClient
    {
        /// <summary>
        /// بازارهای جریان (Flow) که دریافت می‌شوند: ۱ بورس، ۲ فرابورس، ۴ پایهٔ فرابورس، ۷ کالا
        /// (همان فهرست کد قدیمی).
        /// </summary>
        private static readonly byte[] FlowMarkets = { 1, 2, 4, 7 };

        private readonly IProviderSettings _settings;
        private readonly TsetmcCredentials _credentials;
        private readonly ILogger<TsetmcWebServiceClient> _logger;

        /// <summary>سازنده.</summary>
        public TsetmcWebServiceClient(IProviderSettings settings, TsetmcCredentials credentials, ILogger<TsetmcWebServiceClient> logger)
        {
            _settings = settings;
            _credentials = credentials;
            _logger = logger;
        }

        /// <summary>فهرست ابزارهای مالی (سهام، حق تقدم، اوراق و...).</summary>
        internal Task<IReadOnlyList<XElement>> GetInstrumentsAsync(CancellationToken ct)
            => QueryAsync((c, u, p, f) => c.InstrumentAsync(u, p, f), "Instruments", "TseInstruments", ct);

        /// <summary>آخرین معاملهٔ همهٔ نمادها.</summary>
        internal Task<IReadOnlyList<XElement>> GetTradeLastDayAsync(CancellationToken ct)
            => QueryAsync((c, u, p, f) => c.TradeLastDayAsync(u, p, f), "TradeLastDay", "TradeLastDay", ct);

        /// <summary>محدودهٔ مجاز قیمت امروز.</summary>
        internal Task<IReadOnlyList<XElement>> GetStaticThresholdsAsync(CancellationToken ct)
            => QueryAsync((c, u, p, f) => c.StaticThresholdsAsync(u, p, f), "StaticThresholds", "StaticThresholds", ct);

        /// <summary>پنج سطح برتر صف خرید و فروش همهٔ نمادها.</summary>
        internal Task<IReadOnlyList<XElement>> GetBestLimitsAsync(CancellationToken ct)
            => QueryAsync((c, u, p, f) => c.BestLimitsAllInsAsync(u, p, f), "AllBestLimits", "InstBestLimit", ct);

        private async Task<IReadOnlyList<XElement>> QueryAsync(
            Func<TsePublicV2SoapClient, string, string, byte, Task<ArrayOfXElement>> call,
            string parentName, string childName, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(_credentials.User) || string.IsNullOrWhiteSpace(_credentials.Password))
{
                throw new InvalidOperationException(
                    "TSETMC web service credentials are not configured. Set MarketIntelligence:Providers:Tsetmc:User/Password in User Secrets or environment.");
            }

            var url = await _settings.GetAsync(TseMarket.TsetmcProviderKey, TsetmcSettingDefaults.WebServiceUrlKey, ct)
                      ?? throw new InvalidOperationException("Setting 'WebServiceUrl' of provider 'tsetmc' is not configured.");

            var client = new TsePublicV2SoapClient(TsePublicV2SoapClient.EndpointConfiguration.TsePublicV2Soap12, url);
            try
            {
                var rows = new List<XElement>();

                foreach (var flow in FlowMarkets)
                {
                    ct.ThrowIfCancellationRequested();

                    var response = await call(client, _credentials.User!, _credentials.Password!, flow);

                    // Nodes[0] شمای XML و Nodes[1] داده است. پاسخ بدون Nodes[1] یعنی آن بازار ردیفی ندارد.
                    if (response?.Nodes is { Count: >= 2 } nodes)
                        rows.AddRange(nodes[1].Elements(parentName).Elements(childName));
                }

                return rows;
            }
            finally
            {
                await CloseQuietlyAsync(client);
            }
        }

        private async Task CloseQuietlyAsync(TsePublicV2SoapClient client)
        {
            try
            {
                await client.CloseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Closing TSETMC SOAP client failed; aborting");
                client.Abort();
            }
        }
    }
}
