namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <summary>اجرای یک کار دریافت داده برای یک بازار. هم زمان‌بند و هم API دستی از همین استفاده می‌کنند.</summary>
    public interface IMarketDataJobRunner
    {
        /// <summary>
        /// کار را اجرا می‌کند. خطای منبع به‌صورت استثنا بالا نمی‌آید و در نتیجه می‌آید؛
        /// تنها استثناهای لغو و خطاهای ثبت دیتابیس (مثل قفل برنامه) بالا می‌روند تا زمان‌بند تصمیم بگیرد.
        /// </summary>
        Task<MarketJobResult> RunAsync(string marketKey, MarketJobKind job, CancellationToken cancellationToken);
    }
}
