namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <summary>
    /// قفل «هم‌زمان نبودن» برای هر کلید (بازار + کار). دو اجرای هم‌زمان یک کار برای یک بازار نمی‌شود
    /// (مثلاً اجرای زمان‌بند و اجرای دستی API). توجه: فقط درون یک پروسه معتبر است؛ برای چند نمونهٔ برنامه باید قفل توزیع‌شده (مثلاً sp_getapplock) اضافه شود.
    /// </summary>
    public interface IMarketJobGate
    {
        /// <summary>تلاش بدون انتظار برای گرفتن قفل. اگر قفل در دست دیگری بود null برمی‌گرداند؛ در غیر این صورت با Dispose آزاد می‌شود.</summary>
        IDisposable? TryEnter(string marketKey, MarketJobKind job);
    }
}
