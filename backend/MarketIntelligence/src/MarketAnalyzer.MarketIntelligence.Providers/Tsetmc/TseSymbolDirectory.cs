using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>نمادی از بازار بورس با کلید استاندارد و شناسهٔ TSETMC آن (InsCode).</summary>
    /// <param name="SymbolKey">کلید استاندارد نماد (InstrumentId).</param>
    /// <param name="InsCode">کد نماد در TSETMC.</param>
    public readonly record struct TseSymbolRef(string SymbolKey, string InsCode);

    /// <summary>
    /// نگاشت نمادهای فعال بورس بین «کلید استاندارد» و «InsCode». سرویس‌های TSETMC فقط InsCode می‌دهند؛
    /// کلید استاندارد از کاتالوگ ذخیره‌شده می‌آید. نمونه Scoped است و نتیجه را در طول یک اجرا cache می‌کند.
    /// </summary>
    public sealed class TseSymbolDirectory
    {
        private readonly IMarketDefinitionStore _markets;
        private readonly IMarketSymbolStore _symbols;
        private IReadOnlyList<TseSymbolRef>? _active;

        /// <summary>سازنده.</summary>
        public TseSymbolDirectory(IMarketDefinitionStore markets, IMarketSymbolStore symbols)
        {
            _markets = markets;
            _symbols = symbols;
        }

        /// <summary>نمادهای فعال (عضو دنیای پیگیری‌شده) که InsCode دارند.</summary>
        public async Task<IReadOnlyList<TseSymbolRef>> GetActiveAsync(CancellationToken ct)
        {
            if (_active != null)
                return _active;

            var market = await _markets.GetByKeyAsync(TseMarket.Key, ct);
            if (market == null)
                return _active = Array.Empty<TseSymbolRef>();

            var symbols = await _symbols.GetByMarketAsync(market.Id, onlyActive: true, ct);
            return _active = symbols
                .Where(s => !string.IsNullOrWhiteSpace(s.ExternalId))
                .Select(s => new TseSymbolRef(s.SymbolKey, s.ExternalId!))
                .ToList();
        }

        /// <summary>نگاشت InsCode به کلید استاندارد برای نمادهای فعال.</summary>
        public async Task<IReadOnlyDictionary<string, string>> GetInsCodeMapAsync(CancellationToken ct)
        {
            var active = await GetActiveAsync(ct);
            var map = new Dictionary<string, string>(active.Count);
            foreach (var s in active)
                map[s.InsCode] = s.SymbolKey;
            return map;
        }
    }
}
