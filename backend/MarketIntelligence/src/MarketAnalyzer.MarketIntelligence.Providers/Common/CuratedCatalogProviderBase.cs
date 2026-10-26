using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// پایهٔ Providerهایی که فهرست نمادشان را یک فهرست ثابت و دست‌چین داخل DLL تعیین می‌کند
    /// (بازارهایی که منبعی برای کشف خودکار نماد ندارند). فهرست کامل است، پس نمادی که از فهرست حذف شود غیرفعال می‌شود.
    /// </summary>
    public abstract class CuratedCatalogProviderBase : ISymbolCatalogProvider
    {
        /// <summary>کلید Provider فهرست‌های ثابت.</summary>
        public const string CuratedProviderKey = "curated-list";

        /// <inheritdoc />
        public abstract ProviderDescriptor Descriptor { get; }

        /// <summary>فهرست نمادها را از منبع ثابت می‌سازد.</summary>
        protected abstract IReadOnlyList<SymbolInfo> BuildSymbols();

        /// <inheritdoc />
        public Task<FetchResult<IReadOnlyList<SymbolInfo>>> FetchSymbolsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var list = BuildSymbols();
                return Task.FromResult(list.Count == 0
                    ? FetchResult<IReadOnlyList<SymbolInfo>>.SuccessEmpty()
                    : FetchResult<IReadOnlyList<SymbolInfo>>.Success(list, list.Count));
            }
            catch (Exception ex)
            {
                return Task.FromResult(FetchResult<IReadOnlyList<SymbolInfo>>.Failed($"Curated symbol list failed: {ex.Message}", ex));
            }
        }
    }
}
