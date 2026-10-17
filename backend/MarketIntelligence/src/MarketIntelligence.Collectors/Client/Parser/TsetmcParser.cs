
using HtmlAgilityPack;
using System.Globalization;
using System.Text.RegularExpressions;

public class TsetmcParser
{
    public InstrumentInfo Parse(string html, string url)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var text = HtmlEntity.DeEntitize(doc.DocumentNode.InnerText);

        var info = new InstrumentInfo
        {
            Url = url
        };

        info.LastTradePrice = ExtractDecimal(text, @"آخرین معامله\s*\|\s*([\d,]+)");
        info.ClosingPrice = ExtractDecimal(text, @"قیمت پایانی\s*\|\s*([\d,]+)");
        info.FirstPrice = ExtractDecimal(text, @"اولین قیمت\s*\|\s*([\d,]+)");
        info.YesterdayPrice = ExtractDecimal(text, @"قیمت دیروز\s*\|\s*([\d,]+)");

        var day = ExtractTwo(text, @"بازه روز\s*\|\s*([\d,]+)\s*\|\s*([\d,]+)");
        if (day != null)
        {
            info.DayHigh = day.Value.Item1;
            info.DayLow = day.Value.Item2;
        }

        var week = ExtractTwo(text, @"بازه هفته\s*\|\s*([\d,]+)\s*\|\s*([\d,]+)");
        if (week != null)
        {
            info.WeekHigh = week.Value.Item1;
            info.WeekLow = week.Value.Item2;
        }

        var year = ExtractTwo(text, @"بازه سال\s*\|\s*([\d,]+)\s*\|\s*([\d,]+)");
        if (year != null)
        {
            info.YearHigh = year.Value.Item1;
            info.YearLow = year.Value.Item2;
        }

        info.TradeCount = ExtractLong(text, @"تعداد معاملات\s*\|\s*([\d,]+)");
        info.Volume = ExtractLong(text, @"حجم معاملات\s*\|\s*([\d,]+)");

        info.MarketValue = ExtractRaw(text, @"ارزش بازار\s*\|\s*(.+)");
        info.Turnover = ExtractRaw(text, @"ارزش معاملات\s*\|\s*(.+)");

        info.ShareCount = ExtractLong(text, @"تعداد سهام\s*\|\s*([\d,]+)");

        info.EPS = ExtractDecimal(text, @"EPS:\s*\|\s*([\d,]+)");
        info.PE = ExtractDecimal(text, @"P/E:\s*\|\s*([\d.]+)");
        info.GroupPE = ExtractDecimal(text, @"P/Eگروه:\s*\|\s*([\d.]+)");
        info.PS = ExtractDecimal(text, @"P/S:\s*\|\s*([\d.]+)");

        info.Notices = ParseNotices(text);

        return info;
    }

    private decimal? ExtractDecimal(string text, string pattern)
    {
        var m = Regex.Match(text, pattern);

        if (!m.Success)
            return null;

        var raw = m.Groups[1].Value.Replace(",", "");

        if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            return value;

        return null;
    }

    private long? ExtractLong(string text, string pattern)
    {
        var m = Regex.Match(text, pattern);

        if (!m.Success)
            return null;

        var raw = m.Groups[1].Value.Replace(",", "");

        if (long.TryParse(raw, out var value))
            return value;

        return null;
    }

    private string ExtractRaw(string text, string pattern)
    {
        var m = Regex.Match(text, pattern);

        return m.Success ? m.Groups[1].Value.Trim() : null;
    }

    private (decimal, decimal)? ExtractTwo(string text, string pattern)
    {
        var m = Regex.Match(text, pattern);

        if (!m.Success)
            return null;

        var a = decimal.Parse(m.Groups[1].Value.Replace(",", ""));
        var b = decimal.Parse(m.Groups[2].Value.Replace(",", ""));

        return (a, b);
    }

    private List<NoticeItem> ParseNotices(string text)
    {
        var list = new List<NoticeItem>();

        var matches = Regex.Matches(text, @"(\d{4}/\d{2}/\d{2})\s*\n\s*(.+)");

        foreach (Match m in matches)
        {
            list.Add(new NoticeItem
            {
                Date = m.Groups[1].Value,
                Title = m.Groups[2].Value.Trim()
            });
        }

        return list;
    }
}
