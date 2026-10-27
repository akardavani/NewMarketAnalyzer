namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <summary>نتیجهٔ درخواست تعویض منبع.</summary>
    public enum ProviderSwitchStatus
    {
        /// <summary>منبع عوض شد.</summary>
        Switched = 1,

        /// <summary>منبع از قبل همین بود؛ تغییری نشد.</summary>
        NoChange = 2,

        /// <summary>تعویض کندل‌ها همهٔ کندل‌های بازار را پاک می‌کند؛ بدون تأیید (confirmWipe) انجام نشد.</summary>
        ConfirmationRequired = 3,

        /// <summary>اجرای کار این بازار در جریان است؛ بعداً دوباره تلاش کن.</summary>
        Busy = 4,

        /// <summary>درخواست نامعتبر است (بازار یا Provider ناشناخته یا ناسازگار).</summary>
        Invalid = 5
    }

    /// <summary>پاسخ تعویض منبع.</summary>
    public sealed record ProviderSwitchResult(ProviderSwitchStatus Status, string Message, int DeletedCandles = 0);
}
