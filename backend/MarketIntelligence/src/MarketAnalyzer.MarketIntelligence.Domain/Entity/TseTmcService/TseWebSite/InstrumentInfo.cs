public class InstrumentInfo
{
    public string Url { get; set; }

    public decimal? LastTradePrice { get; set; }
    public decimal? ClosingPrice { get; set; }
    public decimal? FirstPrice { get; set; }
    public decimal? YesterdayPrice { get; set; }

    public decimal? DayLow { get; set; }
    public decimal? DayHigh { get; set; }

    public decimal? WeekLow { get; set; }
    public decimal? WeekHigh { get; set; }

    public decimal? YearLow { get; set; }
    public decimal? YearHigh { get; set; }

    public long? TradeCount { get; set; }
    public long? Volume { get; set; }

    public string Turnover { get; set; }
    public string MarketValue { get; set; }

    public long? ShareCount { get; set; }

    public decimal? EPS { get; set; }
    public decimal? PE { get; set; }
    public decimal? GroupPE { get; set; }
    public decimal? PS { get; set; }

    public List<TradeHistoryItem> History { get; set; } = new();
    public List<NoticeItem> Notices { get; set; } = new();
}
