using MarketAnalyzer.Collector;
using MarketIntelligence.Domain;
using Microsoft.Extensions.Options;

public sealed class TseCandleSource : NahayatnegarCandleSourceBase
{
    public TseCandleSource(IOptions<UrlSettings> settings, IDataFetcher fetcher)
        : base(settings, fetcher)
    {
    }

    public override MarketType MarketType => MarketType.Tse;
    public override SourceType SourceType => SourceType.Nahayatnegar;
}
