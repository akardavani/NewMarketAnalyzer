using MarketAnalyzer.Collector;
using MarketIntelligence.Domain;
using Microsoft.Extensions.Options;

public sealed class TseIndexCandleSource : NahayatnegarCandleSourceBase
{
    public TseIndexCandleSource(IOptions<UrlSettings> settings, IDataFetcher fetcher)
        : base(settings, fetcher)
    {
    }

    public override MarketType MarketType => MarketType.TseIndex;
    public override SourceType SourceType => SourceType.Nahayatnegar;
}
