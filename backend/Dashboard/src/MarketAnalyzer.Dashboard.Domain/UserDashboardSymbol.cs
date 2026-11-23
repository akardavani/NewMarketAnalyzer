namespace MarketAnalyzer.Dashboard.Domain;

/// <summary>A symbol selected by a user for their personal dashboard.</summary>
public sealed class UserDashboardSymbol
{
    private UserDashboardSymbol() { }

    public UserDashboardSymbol(int userId, string marketKey, string symbolKey, int displayOrder)
    {
        UserId = userId;
        MarketKey = marketKey;
        SymbolKey = symbolKey;
        DisplayOrder = displayOrder;
    }

    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string MarketKey { get; private set; } = string.Empty;
    public string SymbolKey { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
}
