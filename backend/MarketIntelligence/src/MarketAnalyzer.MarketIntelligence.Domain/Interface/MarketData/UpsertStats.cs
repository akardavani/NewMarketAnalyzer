namespace MarketAnalyzer.MarketIntelligence.Domain
{
    /// <summary>نتیجهٔ یک Upsert: چند رکورد جدید ساخته شد و چند رکورد از قبل وجود داشت (و با مقدار جدید هم‌تراز شد).</summary>
    /// <param name="Inserted">تعداد رکوردهای جدید.</param>
    /// <param name="Existing">تعداد رکوردهایی که از قبل وجود داشتند.</param>
    public readonly record struct UpsertStats(int Inserted, int Existing);
}
