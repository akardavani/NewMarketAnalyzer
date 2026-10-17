using System.Globalization;
using System.Text.RegularExpressions;

public class IncomeStatementContentParser
{
    public IncomeStatementReport Parse(TsetmcCodalStatementItem item)
    {
        var report = new IncomeStatementReport
        {
            Title = item.Title,
            PublishDateTimeGregorian = item.PublishDateTimeGregorian,
            PublishDateTimeDEven = item.PublishDateTimeDEven
        };

        if (string.IsNullOrWhiteSpace(item.Content))
            return report;

        var normalized = NormalizeText(item.Content);

        ExtractMeta(normalized, report);

        var lines = normalized
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        var tableHeaderIndex = lines.FindIndex(x => x.StartsWith("شرح |"));
        if (tableHeaderIndex < 0)
            return report;

        var headerLine = lines[tableHeaderIndex];
        var dataLines = lines.Skip(tableHeaderIndex + 1).ToList();

        report.Columns = ParseColumns(headerLine);
        report.Rows = ParseRows(dataLines, report.Columns.Count);

        report.Metrics = ExtractMetrics(report.Rows);

        return report;
    }

    private void ExtractMeta(string content, IncomeStatementReport report)
    {
        var firstLine = content
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(firstLine))
            return;

        var dateMatches = Regex.Matches(firstLine, @"14\d{2}/\d{2}/\d{2}")
            .Select(x => x.Value)
            .ToList();

        if (dateMatches.Count > 0)
            report.FiscalPeriodEnd = dateMatches.Last();

        var monthMatch = Regex.Match(firstLine, @"(\d{1,2})");
        if (monthMatch.Success)
            report.ReportLengthMonths = monthMatch.Groups[1].Value;

        if (firstLine.Contains("حسابرسی شده"))
            report.AuditStatus = "حسابرسی شده";
        else if (firstLine.Contains("حسابرسی نشده"))
            report.AuditStatus = "حسابرسی نشده";
    }

    private List<IncomeStatementColumn> ParseColumns(string headerLine)
    {
        return headerLine
            .Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Skip(1) // حذف "شرح"
            .Select(x => new IncomeStatementColumn
            {
                Header = NormalizeWhitespace(x)
            })
            .ToList();
    }

    private List<IncomeStatementRow> ParseRows(List<string> lines, int columnCount)
    {
        var rows = new List<IncomeStatementRow>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split('|').Select(NormalizeWhitespace).ToList();

            if (parts.Count < 2)
                continue;

            var row = new IncomeStatementRow
            {
                ItemTitle = parts[0]
            };

            foreach (var valueText in parts.Skip(1).Take(columnCount))
            {
                row.Values.Add(ParseNullableDecimal(valueText));
            }

            while (row.Values.Count < columnCount)
                row.Values.Add(null);

            rows.Add(row);
        }

        return rows;
    }

    private IncomeStatementKeyMetrics ExtractMetrics(List<IncomeStatementRow> rows)
    {
        decimal? Find(params string[] keys)
        {
            var row = rows.FirstOrDefault(r => keys.Any(k => NormalizeWhitespace(r.ItemTitle) == NormalizeWhitespace(k)));
            return row?.Values.FirstOrDefault();
        }

        return new IncomeStatementKeyMetrics
        {
            Revenue = Find("درآمدهای عملیاتی"),
            CostOfRevenue = Find("بهاى تمام شده درآمدهای عملیاتی"),
            GrossProfit = Find("سود(زیان) ناخالص"),
            OperatingProfit = Find("سود(زیان) عملیاتى"),
            ProfitBeforeTax = Find("سود(زیان) عملیات در حال تداوم قبل از مالیات"),
            NetProfit = Find("سود(زیان) خالص"),
            EpsRial = Find("سود (زیان) خالص هر سهم – ریال", "سود (زیان) خالص هر سهم - ریال"),
            Capital = Find("سرمایه")
        };
    }

    private static decimal? ParseNullableDecimal(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var cleaned = input
            .Replace(",", "")
            .Replace("(", "-")
            .Replace(")", "")
            .Trim();

        if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
            return val;

        return null;
    }

    private static string NormalizeText(string text)
    {
        text = text.Replace("\\n", "\n");
        text = text.Replace("\r", "");
        return text;
    }

    private static string NormalizeWhitespace(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return Regex.Replace(input, @"\s+", " ").Trim();
    }
}
