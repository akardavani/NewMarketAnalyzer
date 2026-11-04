namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public sealed class ImeSymbolMetadata
    {
        /// <summary>
        /// تاریخ فارسی
        /// </summary>
        public string PersianDeliveryDate { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateOnly DeliveryDate { get; set; }

        public string? Parent { get; set; }

        public string? ParentSymbol { get; set; }
    }
}


