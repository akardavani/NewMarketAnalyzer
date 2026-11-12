using System;
using System.Collections.Generic;

namespace MarketAnalyzer.Api.Models;

public sealed record DashboardSymbolDto
{
    public required string MarketKey { get; init; }
    public required string SymbolKey { get; init; }
    public required string Name { get; init; }
    public required string CurrencyCode { get; init; }
    public required string PriceLabel { get; init; }
    public decimal? LastPrice { get; init; }
    public DateTime? PriceTimestampUtc { get; init; }
    public int DisplayOrder { get; init; }
}

public sealed record DashboardSymbolSelection
{
    public required string MarketKey { get; init; }
    public required string SymbolKey { get; init; }
}

public sealed record SaveDashboardSettingsRequest
{
    public IReadOnlyList<DashboardSymbolSelection> Symbols { get; init; } = Array.Empty<DashboardSymbolSelection>();
}

public sealed record DashboardSymbolSearchDto
{
    public required int SymbolId { get; init; }
    public required string MarketKey { get; init; }
    public required string SymbolKey { get; init; }
    public required string Name { get; init; }
    public required string CurrencyCode { get; init; }
}
