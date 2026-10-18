namespace MarketIntelligence.Domain;

public class UrlSetting
{
    public int Id { get; set; }
    public string Section { get; set; } = "";
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";
}

public interface IUrlSettingService
{
    Task<bool> HasAnyAsync(CancellationToken ct);
    Task<IReadOnlyList<UrlSetting>> GetAllAsync(CancellationToken ct);
    Task SaveAsync(IReadOnlyList<UrlSetting> items, CancellationToken ct);
}
