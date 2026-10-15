namespace MarketAnalyzer.Domain
{
    public class UnifiedSymbolRecord : IEntity
    {
        public string Id { get; set; }

        public MarketType MarketId { get; set; }
        public MarketRecord Market { get; set; } = null!;

        public string SymbolKey { get; set; } = null!;

        public string? Symbol { get; set; }

        public string? InstrumentId { get; set; }

        public string? InsCode { get; set; }

        public string? Name { get; set; }

        public string? PersianName { get; set; }

        public string? ParentSymbol { get; set; }

        public bool IsActive { get; set; } = true;

        public string? MetadataJson { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
