using HtmlAgilityPack;
using MarketIntelligence.Domain.Entity;
using System.Text.RegularExpressions;
using System.Xml.Linq;

public class FinancialMapper
{
    private static readonly Dictionary<string, string> MetricMap = new()
    {
        { "جمع دارایی‌های جاری", "CurrentAssets" },
        { "جمع داراییها", "TotalAssets" },
        { "جمع بدهی‌های جاری", "CurrentLiabilities" },
        { "جمع بدهیها", "TotalLiabilities" },
        { "حقوق صاحبان سهام", "Equity" },
        { "سرمایه", "Capital" },

        { "درآمدهای عملیاتی", "Revenue" },
        { "سود (زیان) ناخالص", "GrossProfit" },
        { "سود (زیان) عملیاتی", "OperatingProfit" },
        { "سود (زیان) خالص", "NetProfit" },
        { "سود (زیان) خالص هر سهم", "EPS" }
    };

    //public StandardFinancialData Normalize(
    //    long insCode,
    //    IFinancialReport report)
    //{
    //    var result = new StandardFinancialData
    //    {
    //        InsCode = insCode,
    //        ReportTitle = report.Title
    //    };

    //    if (report is CodalStatementDto codal)
    //    {
    //        result.PageId = codal.PageId;
    //        result.PublishDate = codal.PublishDateTimeGregorian;
    //        result.SentDate = codal.SentDateTimeGregorian;
    //        result.PublishDateDEven = codal.PublishDateTimeDEven;
    //        result.ReportSubType = codal.ReportSubType;
    //    }

    //    if (string.IsNullOrWhiteSpace(report.Content))
    //        return result;

    //    ParseHtmlTable(report.Content, result);

    //    return result;
    //}

    //private void ParseHtmlTable(
    //    string html,
    //    StandardFinancialData result)
    //{
    //    var doc = new HtmlDocument();
    //    doc.LoadHtml(html);

    //    var rows = doc.DocumentNode.SelectNodes("//tr");

    //    if (rows == null)
    //        return;

    //    foreach (var row in rows)
    //    {
    //        var cells = row.SelectNodes("td");

    //        if (cells == null || cells.Count < 2)
    //            continue;

    //        string title =
    //            HtmlEntity.DeEntitize(cells[0].InnerText)
    //                .Trim();

    //        foreach (var map in MetricMap)
    //        {
    //            if (!title.Contains(map.Key))
    //                continue;

    //            decimal? value = ExtractNumericValue(cells);

    //            if (value.HasValue)
    //            {
    //                result.Metrics[map.Value] = value;
    //            }
    //        }
    //    }
    //}

    //private decimal? ExtractNumericValue(HtmlNodeCollection cells)
    //{
    //    // از آخر به اول می‌گردیم
    //    // چون معمولاً ستون آخر جدیدترین مقدار است

    //    for (int i = cells.Count - 1; i >= 1; i--)
    //    {
    //        string text =
    //            HtmlEntity.DeEntitize(cells[i].InnerText)
    //                .Trim();

    //        if (string.IsNullOrWhiteSpace(text))
    //            continue;

    //        text = text
    //            .Replace(",", "")
    //            .Replace("(", "-")
    //            .Replace(")", "");

    //        if (decimal.TryParse(text, out var value))
    //            return value;
    //    }

    //    return null;
    //}
}

public class FinancialRow
{
    public string Title { get; set; } = "";

    public List<decimal?> Values { get; set; } = new();
}

public class ParsedFinancialReport
{
    public List<FinancialRow> Rows { get; set; } = new();
}
public interface IReportParser
{
    bool CanParse(string content);

    ParsedFinancialReport Parse(string content);
}



public class XmlReportParser : IReportParser
{
    public bool CanParse(string content)
    {
        return content.Contains("<Row>")
            || content.Contains("<?xml");
    }

    public ParsedFinancialReport Parse(string content)
    {
        var result = new ParsedFinancialReport();

        var doc = XDocument.Parse(content);

        var rows = doc.Descendants("Row");

        foreach (var row in rows)
        {
            var cells = row.Descendants("Cell").ToList();

            if (cells.Count < 2)
                continue;

            var financialRow = new FinancialRow
            {
                Title = cells[0].Value.Trim()
            };

            for (int i = 1; i < cells.Count; i++)
            {
                financialRow.Values.Add(
                    ParseDecimal(cells[i].Value));
            }

            result.Rows.Add(financialRow);
        }

        return result;
    }

    private decimal? ParseDecimal(string text)
    {
        text = text
            .Replace(",", "")
            .Replace("(", "-")
            .Replace(")", "")
            .Trim();

        return decimal.TryParse(text, out var d)
            ? d
            : null;
    }
}


public class HtmlReportParser : IReportParser
{
    public bool CanParse(string content)
    {
        return content.Contains("<table")
            || content.Contains("<tr");
    }

    public ParsedFinancialReport Parse(string content)
    {
        var result = new ParsedFinancialReport();

        var doc = new HtmlDocument();

        doc.LoadHtml(content);

        var rows = doc.DocumentNode.SelectNodes("//tr");

        if (rows == null)
            return result;

        foreach (var row in rows)
        {
            var cells = row.SelectNodes("td");

            if (cells == null || cells.Count < 2)
                continue;

            var financialRow = new FinancialRow
            {
                Title = cells[0].InnerText.Trim()
            };

            for (int i = 1; i < cells.Count; i++)
            {
                financialRow.Values.Add(
                    ParseDecimal(cells[i].InnerText));
            }

            result.Rows.Add(financialRow);
        }

        return result;
    }

    private decimal? ParseDecimal(string text)
    {
        text = text
            .Replace(",", "")
            .Replace("(", "-")
            .Replace(")", "")
            .Trim();

        return decimal.TryParse(text, out var d)
            ? d
            : null;
    }
}


public class TextReportParser : IReportParser
{
    public bool CanParse(string content)
    {
        return true;
    }

    public ParsedFinancialReport Parse(string content)
    {
        var result = new ParsedFinancialReport();

        var lines = content.Split('\n');

        foreach (var line in lines)
        {
            var matches = Regex.Matches(
                line,
                @"-?\d[\d,]*");

            if (matches.Count == 0)
                continue;

            var row = new FinancialRow
            {
                Title = line
            };

            foreach (Match match in matches)
            {
                if (decimal.TryParse(
                    match.Value.Replace(",", ""),
                    out var value))
                {
                    row.Values.Add(value);
                }
            }

            result.Rows.Add(row);
        }

        return result;
    }
}

public class ParserFactory
{
    private readonly List<IReportParser> _parsers;

    public ParserFactory()
    {
        _parsers = new()
        {
            new XmlReportParser(),
            new HtmlReportParser(),
            new TextReportParser()
        };
    }

    public IReportParser Resolve(string content)
    {
        return _parsers.First(x => x.CanParse(content));
    }
}
public static class FinancialKeywords
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "جمع دارایی", "TotalAssets" },
        { "دارایی جاری", "CurrentAssets" },
        { "جمع بدهی", "TotalLiabilities" },
        { "بدهی جاری", "CurrentLiabilities" },
        { "حقوق صاحبان سهام", "Equity" },

        { "درآمد عملیاتی", "Revenue" },
        { "سود ناخالص", "GrossProfit" },
        { "سود عملیاتی", "OperatingProfit" },
        { "سود خالص", "NetProfit" },
        { "EPS", "EPS" }
    };
}

public class FinancialMetricMapper
{
    public StandardFinancialData Map(
        long insCode,
        CodalStatementDto dto,
        ParsedFinancialReport parsed)
    {
        var result = new StandardFinancialData
        {
            InsCode = insCode,
            ReportTitle = dto.Title,
            PublishDate = dto.PublishDateTimeGregorian,
            SentDate = dto.SentDateTimeGregorian,
            PublishDateDEven = dto.PublishDateTimeDEven,
            ReportSubType = dto.ReportSubType
        };

        foreach (var row in parsed.Rows)
        {
            foreach (var map in FinancialKeywords.Map)
            {
                if (!row.Title.Contains(map.Key))
                    continue;

                var latestValue =
                    row.Values.LastOrDefault();

                if (latestValue.HasValue)
                {
                    result.Metrics[map.Value]
                        = latestValue.Value;
                }
            }
        }

        return result;
    }
}

public class FinancialReportService
{
    private readonly ParserFactory _factory;
    private readonly FinancialMetricMapper _mapper;

    public FinancialReportService()
    {
        _factory = new ParserFactory();
        _mapper = new FinancialMetricMapper();
    }

    public StandardFinancialData Process(
        long insCode,
        CodalStatementDto dto)
    {
        var parser = _factory.Resolve(dto.Content);

        var parsed = parser.Parse(dto.Content);

        return _mapper.Map(
            insCode,
            dto,
            parsed);
    }
}
