using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Operations
{
    /// <summary>منبع فعال و منابع ممکن برای یک قابلیت.</summary>
    public sealed record CapabilityOverview(ProviderCapability Capability, string? ActiveProviderKey, IReadOnlyList<string> AvailableProviderKeys);

    /// <summary>نمای کلی یک بازار برای صفحهٔ تنظیمات.</summary>
    public sealed record MarketOverview(
        string Key, string Name, string PersianName, string CurrencyCode, bool IsActive,
        IReadOnlyList<CapabilityOverview> Capabilities, IReadOnlyDictionary<string, string> Settings);

    /// <summary>یک تنظیم.</summary>
    public sealed record SettingItem(string Scope, string Key, string Value);
}
