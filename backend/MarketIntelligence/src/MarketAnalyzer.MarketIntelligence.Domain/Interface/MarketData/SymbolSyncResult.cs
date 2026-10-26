namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>نتیجهٔ همگام‌سازی کاتالوگ نمادهای یک بازار.</summary>
    /// <param name="Inserted">نمادهای جدید.</param>
    /// <param name="Updated">نمادهای موجود که مشخصاتشان تغییر کرد.</param>
    /// <param name="Deactivated">نمادهایی که دیگر در منبع نیستند و غیرفعال شدند (هرگز حذف نمی‌شوند).</param>
    public readonly record struct SymbolSyncResult(int Inserted, int Updated, int Deactivated);
}
