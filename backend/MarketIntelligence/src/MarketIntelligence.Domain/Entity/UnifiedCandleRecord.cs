namespace MarketIntelligence.Domain
{
    public class UnifiedCandleRecord 
    {
        public string Id { get; set; }
        // برای اینکه همه بازارها یک جدول داشته باشند
        public MarketType MarketId { get; set; }
        // زمان‌بندی
        public Timeframe TimeframeId { get; set; }
        // نماد
        public string SymbolKey { get; set; } = null!;

        // داده‌های کندل
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Value { get; set; }

        public DateTime Date { get; set; }
        public string PersianDate { get; set; } = "";

        public DateTime CreatedAtUtc { get; set; }

        public MarketRecord Market { get; set; } = null!;
        public TimeframeRecord Timeframe { get; set; } = null!;

        public void GenerateId()
        {
            var unixTimestamp = new DateTimeOffset(Date.ToUniversalTime()).ToUnixTimeSeconds();
            Id = $"{unixTimestamp}_{(int)MarketId}_{(int)TimeframeId}_{SymbolKey}";
        }

        //public void GenerateId()
        //{
        //    var unixTimestamp = new DateTimeOffset(Date).ToUnixTimeSeconds();
        //    var normalizedSymbolKey = SymbolKey?.Trim().ToUpperInvariant();

        //    var raw = $"{unixTimestamp}|{(int)MarketId}|{(int)TimeframeId}|{normalizedSymbolKey}";
        //    Id = ComputeSha256(raw);
        //}
        //private static string ComputeSha256(string input)
        //{
        //    using var sha = System.Security.Cryptography.SHA256.Create();
        //    var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        //    var hash = sha.ComputeHash(bytes);
        //    return Convert.ToHexString(hash);
        //}
    }
}
