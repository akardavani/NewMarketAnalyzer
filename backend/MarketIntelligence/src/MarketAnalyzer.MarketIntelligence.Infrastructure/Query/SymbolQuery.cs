using MarketAnalyzer.MarketIntelligence.Domain;

public class SymbolQuery
{
    public IReadOnlyCollection<MarketType>? Markets { get; set; }

    public IReadOnlyCollection<string>? SymbolKeys { get; set; }

    public IReadOnlyCollection<string>? Symbols { get; set; }

    public IReadOnlyCollection<string>? InstrumentIds { get; set; }

    public IReadOnlyCollection<string>? InsCodes { get; set; }

    public bool? IsActive { get; set; }

    public string? SearchText { get; set; }
}

