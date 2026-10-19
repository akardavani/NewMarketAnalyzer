using System;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MarketAnalyzer.Fundamental;


public static class FinancialStatementParser
{
    public static FinancialStatement Parse(CodalStatementPage report)
    {
        var result = new FinancialStatement
        {
            Title = report.Title
        };

        var doc = XDocument.Parse(report.Content);

        var rows = doc.Descendants("tr").ToList();

        if (!rows.Any())
            return result;

        // Header
        result.Headers = rows.First()
                             .Elements("td")
                             .Select(x => x.Value.Trim())
                             .ToList();

        // Data
        foreach (var row in rows.Skip(1))
        {
            var cells = row.Elements("td")
                           .Select(x => x.Value.Trim())
                           .ToList();

            if (cells.Count == 0)
                continue;

            var item = new FinancialRow
            {
                Name = cells[0]
            };

            for (int i = 1; i < cells.Count; i++)
                item.Values[i] = cells[i];

            result.Rows.Add(item);
        }

        return result;
    }
}

public sealed class CodalStatementResponse
{
    [JsonPropertyName("statemetnContent")]
    public List<CodalStatementPage> Pages { get; set; } = [];
}

public sealed class CodalStatementPage
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("content")]
    public string Content { get; set; } = "";

    [JsonPropertyName("pageID")]
    public int PageId { get; set; }

    [JsonPropertyName("reportSubType")]
    public int ReportSubType { get; set; }

    [JsonPropertyName("publishDateTime_Gregorian")]
    public DateTime PublishDateTime_Gregorian { get; set; }

    [JsonPropertyName("publishDateTime_DEven")]
    public long PublishDateTime_DEven { get; set; }

}

//public class FinancialReportResponse
//{
//    [JsonPropertyName("statemetnContent")]
//    public List<FinancialReportItem> StatemetnContent { get; set; } = [];
//}

//public class FinancialReportItem
//{
//    [JsonPropertyName("title")]
//    public string Title { get; set; } = "";

//    [JsonPropertyName("publishDateTime_Gregorian")]
//    public DateTime PublishDateTime_Gregorian { get; set; }

//    [JsonPropertyName("publishDateTime_DEven")]
//    public long PublishDateTime_DEven { get; set; }

//    [JsonPropertyName("reportSubType")]
//    public int ReportSubType { get; set; }

//    [JsonPropertyName("pageID")]
//    public int PageID { get; set; }

//    [JsonPropertyName("content")]
//    public string Content { get; set; } = "";
//}
public class FinancialStatement
{
    public string Title { get; set; } = "";

    public List<string> Headers { get; set; } = [];

    public List<FinancialRow> Rows { get; set; } = [];
}

public class FinancialRow
{
    public string Name { get; set; } = "";

    public Dictionary<int, string> Values { get; set; } = [];
}

public sealed class ModernStatementLayoutParser
    : StatementLayoutParserBase
{
    public override bool CanParse(CodalXmlDocument document)
    {
        if (document.Tables.Count == 0)
            return false;

        var first = document.Tables.First();

        if (first.Rows.Count == 0)
            return false;

        return first.Rows.First().Cells.Count <= 8;
    }

    protected override CodalXmlTable SelectMainTable(
        CodalXmlDocument document)
    {
        return document.Tables.First();
    }

    protected override GridTable BuildGrid(
    CodalXmlTable table)
    {
        return new GridNormalizer().Normalize(table);
    }
}

public abstract class StatementLayoutParserBase : IStatementLayoutParser
{
    public abstract bool CanParse(CodalXmlDocument document);

    public GridTable Parse(
    CodalStatementPage page,
    CodalXmlDocument document)
    {
        var table = SelectMainTable(document);

        return BuildGrid(table);
    }

    protected abstract CodalXmlTable SelectMainTable(CodalXmlDocument document);

    protected abstract GridTable BuildGrid(
    CodalXmlTable table);
}

public sealed class CodalDocument
{
    public string Title { get; set; } = "";

    public int ReportSubType { get; set; }

    public Dictionary<string, string> Metadata { get; }
        = [];

    public List<CodalTable> Tables { get; }
        = [];
}
public sealed class CodalTable
{
    public string Name { get; set; } = "";

    public GridTable Grid { get; set; }
        = new();
}

public sealed class StatementSection
{
    public string Title { get; set; } = "";

    public List<StatementLine> Lines { get; }
        = [];
}


public sealed class GridTable
{
    public string Name { get; set; } = "";

    public int RowCount => Rows.Count;

    public int ColumnCount =>
    Rows.Count == 0
        ? 0
        : Rows.Max(r =>
            r.Cells.Count == 0
                ? 0
                : r.Cells.Max(c => c.Column) + 1);

    public List<GridRow> Rows { get; } = [];
}

public sealed class GridRow
{
    public int Index { get; set; }

    public List<GridCell> Cells { get; } = [];
}



public interface IGridNormalizer
{
    GridTable Normalize(CodalXmlTable table);
}


public sealed class GridCell
{
    public int Row { get; set; }

    public int Column { get; set; }

    public string Value { get; set; } = "";

    public bool IsHeader { get; set; }
}

public sealed class GridDocument
{
    public List<GridTable> Tables { get; } = [];
}

public interface IStatementLayoutParser
{
    bool CanParse(CodalXmlDocument document);

    GridTable Parse(CodalStatementPage page, CodalXmlDocument document);
}


public sealed class GridNormalizer
{
    public GridTable Normalize(CodalXmlTable source)
    {
        var table = new GridTable();

        int rowIndex = 0;

        foreach (var sourceRow in source.Rows)
        {
            var row = new GridRow
            {
                Index = rowIndex++
            };

            int column = 0;

            foreach (var cell in sourceRow.Cells)
            {
                while (row.Cells.Any(c => c.Column == column))
                    column++;

                AddCell(row, column, cell);

                column += cell.ColSpan;
            }

            table.Rows.Add(row);
        }

        ExpandRowSpans(table);

        return table;
    }


    private static void AddCell(
    GridRow row,
    int column,
    CodalXmlCell cell)
    {
        row.Cells.Add(new GridCell
        {
            Row = row.Index,
            Column = column,
            Value = cell.Value,
            IsHeader = row.Index == 0
        });
    }

    private static void ExpandRowSpans(
    GridTable table)
    {
        foreach (var row in table.Rows)
        {
            foreach (var cell in row.Cells.ToList())
            {
                // اینجا بعداً RowSpan و ColSpan را باز می‌کنیم.
            }
        }
    }

}


//public sealed class GridNormalizer : IGridNormalizer
//{
//    public GridTable Normalize(CodalXmlTable table)
//    {
//        var builder = new GridBuilder();

//        var grid = builder.Build(table);

//        new RowSpanProcessor().Process(grid, table);

//        new ColSpanProcessor().Process(grid, table);

//        new GridCleaner().Clean(grid);

//        return grid;
//    }
//}


public sealed class StatementLayoutDetector
{
    private readonly List<IStatementLayoutParser> _parsers;

    public StatementLayoutDetector()
    {
        _parsers =
        [
            new ModernStatementLayoutParser(),
            new LegacyStatementLayoutParser()
        ];
    }

    public IStatementLayoutParser Detect(
        CodalXmlDocument document)
    {
        foreach (var parser in _parsers)
        {
            if (parser.CanParse(document))
                return parser;
        }

        throw new InvalidOperationException(
            "Unknown statement layout.");
    }
}


public sealed class StatementBuilder
{
    public StatementDocument Build(
        CodalStatementPage page,
        GridTable table)
    {
        var document = new StatementDocument
        {
            Title = page.Title
        };

        var layout = new StatementRecognizer()
            .Recognize(table);

        document.Layout = layout;

        ReadColumns(document, layout);

        ReadRows(document, table, layout);

        return document;
    }

    private static void ReadColumns(
    StatementDocument document,
    StatementLayoutInfo layout)
    {
        foreach (var column in layout.Columns)
        {
            if (column.Type != StatementColumnType.Period)
                continue;

            document.Columns.Add(new StatementColumn
            {
                Index = column.Index,

                Header = column.Header,

                PeriodOrder = column.PeriodOrder,

                PeriodText = column.PeriodText,

                Type = column.Type
            });
        }
    }


    private static void ReadRows(
    StatementDocument document,
    GridTable table,
    StatementLayoutInfo layout)
    {
        foreach (var row in table.Rows.Skip(layout.HeaderRowIndex + 1))
        {
            if (row.Cells.Count == 0)
                continue;

            if (layout.TitleColumnIndex >= row.Cells.Count)
                continue;

            var line = new StatementLine
            {
                Title = row.Cells[layout.TitleColumnIndex].Value
            };

            var classifier = new RowClassifier();

            line.Type =
                classifier.Classify(
                    row,
                    layout);

            foreach (var column in layout.Columns)
            {
                if (column.Type != StatementColumnType.Period)
                    continue;

                var cell = row.Cells
                    .FirstOrDefault(c => c.Column == column.Index);

                line.Values.Add(new StatementValue
                {
                    ColumnIndex = column.Index,
                    Value = cell?.Value
                });
            }

            document.Lines.Add(line);
        }
    }
}


public sealed class CodalStatementReader
{
    private readonly CodalXmlReader _xmlReader = new();
    private readonly StatementLayoutDetector _layoutDetector = new();
    private readonly StatementBuilder _builder = new();

    public StatementDocument Read(CodalStatementPage page)
    {
        var xml = _xmlReader.Read(page.Content);

        var parser = _layoutDetector.Detect(xml);

        var table = parser.Parse(page, xml);

        return _builder.Build(page, table);
    }
}

public sealed class CodalXmlReader
{
    public CodalXmlDocument Read(string xml)
    {
        var xdoc = XDocument.Parse(xml);

        var result = new CodalXmlDocument();

        foreach (var tableElement in xdoc.Descendants("table"))
        {
            var table = ReadTable(tableElement);

            result.Tables.Add(table);
        }

        return result;
    }

    private CodalXmlTable ReadTable(XElement tableElement)
    {
        var table = new CodalXmlTable();

        table.Id = tableElement.Attribute("Id")?.Value;

        foreach (var rowElement in tableElement.Descendants("tr"))
        {
            table.Rows.Add(ReadRow(rowElement));
        }

        return table;
    }

    private CodalXmlRow ReadRow(XElement rowElement)
    {
        var row = new CodalXmlRow();

        foreach (var cellElement in rowElement.Elements("td"))
        {
            row.Cells.Add(ReadCell(cellElement));
        }

        return row;
    }

    private CodalXmlCell ReadCell(XElement cellElement)
    {
        var cell = new CodalXmlCell();

        cell.Value = cellElement.Value.Trim();

        if (int.TryParse(cellElement.Attribute("rowspan")?.Value, out var rs))
            cell.RowSpan = rs;

        if (int.TryParse(cellElement.Attribute("colspan")?.Value, out var cs))
            cell.ColSpan = cs;

        cell.Id =
            cellElement.Attribute("Id")?.Value
            ?? cellElement.Attribute("id")?.Value;

        cell.ClassName =
            cellElement.Attribute("class")?.Value;

        foreach (var attr in cellElement.Attributes())
        {
            cell.Attributes[attr.Name.LocalName] = attr.Value;
        }

        return cell;
    }
}



public sealed class CodalXmlDocument
{
    public List<CodalXmlTable> Tables { get; } = [];
}

public sealed class CodalXmlTable
{
    public string? Id { get; set; }

    public List<CodalXmlRow> Rows { get; } = [];
}

public sealed class CodalXmlRow
{
    public List<CodalXmlCell> Cells { get; } = [];
}




public sealed class StatementColumn
{
    public int Index { get; set; }

    public int PeriodOrder { get; set; }

    public string Header { get; set; } = "";

    public string? PeriodText { get; set; }

    public StatementColumnType Type { get; set; }
}



public sealed class CodalXmlCell
{
    public string Value { get; set; } = "";

    public int RowSpan { get; set; } = 1;

    public int ColSpan { get; set; } = 1;

    public string? Id { get; set; }

    public string? ClassName { get; set; }

    public Dictionary<string, string> Attributes { get; } = [];
}




public enum StatementType
{
    Unknown,

    BalanceSheet,

    IncomeStatement,

    CashFlow,

    Equity
}

public enum StatementLineType
{
    Unknown,

    Header,

    Group,

    Item,

    Total,

    Empty
}

public sealed class GridBuilder
{
    public GridTable Build(CodalXmlTable table)
    {
        var grid = new GridTable();

        int rowIndex = 0;

        foreach (var xmlRow in table.Rows)
        {
            var row = new GridRow
            {
                Index = rowIndex++
            };

            int column = 0;

            foreach (var cell in xmlRow.Cells)
            {
                row.Cells.Add(new GridCell
                {
                    Row = row.Index,
                    Column = column,
                    Value = Clean(cell.Value),
                    IsHeader = row.Index == 0
                });

                column += cell.ColSpan;
            }

            grid.Rows.Add(row);
        }

        return grid;
    }

    private static string Clean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        return value
            .Replace("&nbsp;", " ")
            .Replace('\u00A0', ' ')
            .Trim();
    }
}

public sealed class RowSpanProcessor
{
    public void Process(
        GridTable grid,
        CodalXmlTable xml)
    {

    }
}

public sealed class ColSpanProcessor
{
    public void Process(
        GridTable grid,
        CodalXmlTable xml)
    {

    }
}


public sealed class GridCleaner
{
    public void Clean(GridTable grid)
    {
        RemoveEmptyRows(grid);

        RemoveEmptyColumns(grid);

        NormalizeTexts(grid);
    }

    private void RemoveEmptyRows(GridTable grid)
    {

    }

    private void RemoveEmptyColumns(GridTable grid)
    {

    }

    private void NormalizeTexts(GridTable grid)
    {
        foreach (var row in grid.Rows)
        {
            foreach (var cell in row.Cells)
            {
                cell.Value = Normalize(cell.Value);
            }
        }
    }

    private static string Normalize(string value)
    {
        return value
            .Replace("ي", "ی")
            .Replace("ك", "ک")
            .Trim();
    }
}


public sealed class StatementLayoutInfo
{
    public StatementType StatementType { get; set; }

    public int HeaderRowIndex { get; set; }

    public int TitleColumnIndex { get; set; }

    public bool HasSections { get; set; }

    public bool HasDualTables { get; set; }

    public int DataColumnCount { get; set; }

    public List<StatementColumnInfo> Columns { get; } = [];
}

public sealed class StatementColumnInfo
{
    public int Index { get; set; }

    public string Header { get; set; } = "";

    public StatementColumnType Type { get; set; }

    public int PeriodOrder { get; set; } = -1;

    public string? PeriodText { get; set; }

    public bool IsNumeric { get; set; }
}


public interface IStatementRecognizer
{
    StatementLayoutInfo Recognize(GridTable table);
}

public sealed class StatementRecognizer
    : IStatementRecognizer
{
    public StatementLayoutInfo Recognize(GridTable table)
    {
        var info = new StatementLayoutInfo();

        var headerDetector = new HeaderDetector();

        info.HeaderRowIndex = headerDetector.Detect(table);

        info.TitleColumnIndex = FindTitleColumn(table, info.HeaderRowIndex);

        info.StatementType = DetectStatementType(table);

        info.HasSections = DetectSections(table);

        info.HasDualTables = DetectDualTables(table);

        var columnDetector = new ColumnDetector();

        info.Columns.AddRange(
            columnDetector.Detect(
                table,
                info.HeaderRowIndex));

        info.DataColumnCount =
            info.Columns.Count(c =>
                c.Type == StatementColumnType.Period);

        return info;
    }

    private int FindTitleColumn(
    GridTable table,
    int headerRowIndex)
    {
        if (headerRowIndex < 0 ||
            headerRowIndex >= table.Rows.Count)
        {
            return 0;
        }

        var header = table.Rows[headerRowIndex];

        foreach (var cell in header.Cells)
        {
            var value = cell.Value.Trim();

            if (value.Contains("شرح") ||
                value.Contains("عنوان") ||
                value.Contains("اقلام"))
            {
                return cell.Column;
            }
        }

        return header.Cells.First().Column;
    }

    private StatementType DetectStatementType(GridTable table)
    {
        var text = string.Join(" ",
            table.Rows
                 .SelectMany(r => r.Cells)
                 .Select(c => c.Value ?? string.Empty));

        if (text.Contains("جریان وجوه نقد"))
            return StatementType.CashFlow;

        if (text.Contains("سود و زیان") ||
            text.Contains("صورت سود") ||
            text.Contains("درآمد"))
            return StatementType.IncomeStatement;

        if (text.Contains("حقوق مالکانه"))
            return StatementType.Equity;

        if (text.Contains("دارایی") ||
            text.Contains("بدهی"))
            return StatementType.BalanceSheet;

        return StatementType.Unknown;
    }


    private bool DetectSections(
    GridTable table)
    {
        return table.Rows.Any(r =>
            r.Cells.Any(c =>
                c.Value.Contains("دارایی") ||
                c.Value.Contains("بدهی")));
    }

    private bool DetectDualTables(
    GridTable table)
    {
        return table.ColumnCount > 10;
    }
}

public sealed class LegacyStatementLayoutParser
    : StatementLayoutParserBase
{
    public override bool CanParse(CodalXmlDocument document)
    {
        return false;
    }

    protected override CodalXmlTable SelectMainTable(
        CodalXmlDocument document)
    {
        return document.Tables.First();
    }

    protected override GridTable BuildGrid(
        CodalXmlTable table)
    {
        var normalizer = new GridNormalizer();

        return normalizer.Normalize(table);
    }
}





public interface IHeaderRule
{
    int Score(GridRow row);
}

public sealed class HeaderDateRule
    : IHeaderRule
{
    public int Score(GridRow row)
    {
        int score = 0;

        foreach (var cell in row.Cells)
        {
            if (LooksLikeDate(cell.Value))
                score += 20;
        }

        return score;
    }

    private static bool LooksLikeDate(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        return Regex.IsMatch(
            text,
            @"1[34]\d{2}/\d{2}/\d{2}");
    }


}

public sealed class HeaderTitleRule
    : IHeaderRule
{
    public int Score(GridRow row)
    {
        return row.Cells.Any(c =>
            c.Value.Contains("شرح"))
            ? 40
            : 0;
    }
}

public sealed class HeaderNoteRule
    : IHeaderRule
{
    public int Score(GridRow row)
    {
        return row.Cells.Any(c =>
            c.Value.Contains("یادداشت"))
            ? 20
            : 0;
    }
}

public sealed class HeaderNumberRule
    : IHeaderRule
{
    public int Score(GridRow row)
    {
        int count = row.Cells.Count(c =>
            decimal.TryParse(
                c.Value.Replace(",", ""),
                out _));

        return count > 0
            ? 0
            : 10;
    }
}



public sealed class HeaderDetector
{
    private readonly List<IHeaderRule> _rules =
    [
        new HeaderDateRule(),

        new HeaderTitleRule(),

        new HeaderNoteRule(),

        new HeaderNumberRule()
    ];

    public int Detect(GridTable table)
    {
        int bestRow = 0;

        int bestScore = int.MinValue;

        foreach (var row in table.Rows)
        {
            int score = 0;

            foreach (var rule in _rules)
            {
                score += rule.Score(row);
            }

            if (score > bestScore)
            {
                bestScore = score;

                bestRow = row.Index;
            }
        }

        return bestRow;
    }
}

public enum StatementColumnType
{
    Unknown,

    Title,

    Note,

    Period,

    Description
}

//public enum StatementColumnType
//{
//    Unknown,

//    Title,

//    Note,

//    CurrentPeriod,

//    PreviousPeriod,

//    TwoYearsAgo,

//    ThreeYearsAgo,

//    Description
//}


public interface IColumnDetector
{
    List<StatementColumnInfo> Detect(
        GridTable table,
        int headerRowIndex);
}

public sealed class ColumnDetector
    : IColumnDetector
{
    public List<StatementColumnInfo> Detect(
        GridTable table,
        int headerRowIndex)
    {
        var result = new List<StatementColumnInfo>();

        if (headerRowIndex >= table.Rows.Count)
            return result;

        var header = table.Rows[headerRowIndex];

        foreach (var cell in header.Cells)
        {
            result.Add(new StatementColumnInfo
            {
                Index = cell.Column,
                Header = cell.Value,
                Type = DetectType(cell.Value),
                PeriodText = LooksLikeDate(cell.Value)
                    ? cell.Value
                    : null
            });
        }

        AssignPeriods(result);

        return result;
    }
    private StatementColumnType DetectType(string value)
    {
        if (value.Contains("شرح"))
            return StatementColumnType.Title;

        if (value.Contains("یادداشت"))
            return StatementColumnType.Note;

        if (LooksLikeDate(value))
            return StatementColumnType.Period;

        return StatementColumnType.Unknown;
    }

    private static void AssignPeriods(List<StatementColumnInfo> columns)
    {
        int order = 0;

        foreach (var column in columns.Where(c => c.Type == StatementColumnType.Period))
        {
            column.PeriodOrder = order++;
        }
    }

    private static bool LooksLikeDate(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        return Regex.IsMatch(
            text,
            @"1[34]\d{2}/\d{2}/\d{2}");
    }

    private static DateOnly? ParseDate(string? text)
    {
        return null;
    }
}


public interface IRowClassifier
{
    StatementLineType Classify(
        GridRow row,
        StatementLayoutInfo layout);
}

public sealed class RowClassifier
    : IRowClassifier
{
    public StatementLineType Classify(
        GridRow row,
        StatementLayoutInfo layout)
    {
        if (row.Cells.Count == 0)
            return StatementLineType.Empty;

        var title = row.Cells[layout.TitleColumnIndex].Value;

        if (string.IsNullOrWhiteSpace(title))
            return StatementLineType.Empty;

        if (IsTotal(title))
            return StatementLineType.Total;

        if (IsGroup(title))
            return StatementLineType.Group;

        return StatementLineType.Item;
    }

    private static bool IsTotal(string title)
    {
        return title.StartsWith("جمع")
            || title.StartsWith("مجموع");
    }

    private static bool IsGroup(string title)
    {
        return title.EndsWith(":");
    }
}

public enum StatementItemKey
{
    Unknown = 0,

    CashAndCashEquivalents,

    AccountsReceivable,

    Inventory,

    CurrentAssets,

    NonCurrentAssets,

    TotalAssets,

    CurrentLiabilities,

    NonCurrentLiabilities,

    TotalLiabilities,

    Equity,

    Revenue,

    CostOfRevenue,

    GrossProfit,

    OperatingProfit,

    NetProfit
}

//public interface IStatementMapper
//{
//    StatementItemKey Map(string title);
//}

//public sealed class StatementMapper
//    : IStatementMapper
//{
//    public StatementItemKey Map(string title)
//    {
//        title = Normalize(title);

//        if (Contains(title,
//            "موجودی نقد",
//            "وجه نقد",
//            "وجوه نقد",
//            "موجودی نزد بانک"))
//        {
//            return StatementItemKey.CashAndCashEquivalents;
//        }

//        if (Contains(title,
//            "حساب های دریافتنی",
//            "اسناد دریافتنی",
//            "مطالبات تجاری"))
//        {
//            return StatementItemKey.AccountsReceivable;
//        }

//        if (Contains(title,
//            "موجودی کالا"))
//        {
//            return StatementItemKey.Inventory;
//        }

//        if (Contains(title,
//            "جمع دارایی"))
//        {
//            return StatementItemKey.TotalAssets;
//        }

//        return StatementItemKey.Unknown;
//    }

//    private static string Normalize(string text)
//    {
//        return text
//            .Replace("ي", "ی")
//            .Replace("ك", "ک")
//            .Replace("‌", "")
//            .Trim();
//    }

//    private static bool Contains(
//    string value,
//    params string[] patterns)
//    {
//        foreach (var p in patterns)
//        {
//            if (value.Contains(p))
//                return true;
//        }

//        return false;
//    }
//}

//public interface ITreeBuilder
//{
//    List<FinancialNode> Build(
//        StatementDocument document);
//}

//public sealed class TreeBuilder
//    : ITreeBuilder
//{
//    public List<FinancialNode> Build(
//        StatementDocument document)
//    {
//        var result = new List<FinancialNode>();

//        FinancialNode? currentGroup = null;

//        foreach (var line in document.Lines)
//        {
//            var node = CreateNode(line);

//            switch (line.Type)
//            {
//                case StatementLineType.Group:

//                    currentGroup = node;

//                    result.Add(node);

//                    break;

//                case StatementLineType.Item:

//                    if (currentGroup == null)
//                        result.Add(node);
//                    else
//                        currentGroup.Children.Add(node);

//                    break;

//                case StatementLineType.Total:

//                    result.Add(node);

//                    currentGroup = null;

//                    break;
//            }
//        }

//        return result;
//    }

//    private static FinancialNode CreateNode(
//    StatementLine line)
//    {
//        var node = new FinancialNode
//        {
//            Key = line.Key,

//            Title = line.Title,

//            Type = line.Type
//        };

//        for (int i = 0; i < line.Values.Count; i++)
//        {
//            node.Values[i] =
//                ParseDecimal(line.Values[i]);
//        }

//        return node;
//    }



//}

//public interface IFinancialValidator
//{
//    IEnumerable<ValidationError> Validate(
//        FinancialStatement statement);
//}

//public sealed class ValidationError
//{
//    public string Message { get; set; } = "";

//    public ValidationSeverity Severity { get; set; }

//    public StatementItemKey? Key { get; set; }
//}


//public sealed class FinancialPeriod
//{
//    public int Order { get; set; }

//    public string Caption { get; set; } = "";

//    public string? PeriodText { get; set; }
//}



//public interface IStatementConverter
//{
//    FinancialStatement Convert(
//        StatementDocument document);
//}

//public sealed class StatementConverter
//    : IStatementConverter
//{
//    public FinancialStatement Convert(
//        StatementDocument document)
//    {
//        var result = new FinancialStatement();

//        foreach (var column in document.Columns)
//        {
//            result.Periods.Add(new FinancialPeriod
//            {
//                Order = column.PeriodOrder,
//                Caption = column.Header,
//                PeriodText = column.PeriodText
//            });
//        }

//        foreach (var line in document.Lines)
//        {
//            var fact = new FinancialFact
//            {
//                Key = line.Key,
//                Title = line.Title,
//                LineType = line.Type
//            };

//            for (int i = 0; i < line.Values.Count; i++)
//            {
//                fact.Values[i] = ParseDecimal(line.Values[i]);
//            }

//            result.Facts.Add(fact);
//        }

//        return result;
//    }
//    private static decimal? ParseDecimal(string? value)
//    {
//        if (string.IsNullOrWhiteSpace(value))
//            return null;

//        value = value
//            .Replace(",", "")
//            .Replace("(", "-")
//            .Replace(")", "");

//        if (decimal.TryParse(value, out var number))
//            return number;

//        return null;
//    }
//}

//public sealed class FinancialNode
//{
//    public StatementItemKey Key { get; set; }

//    public string Title { get; set; } = "";

//    public StatementLineType Type { get; set; }

//    public List<FinancialNode> Children { get; } = [];

//    public Dictionary<int, decimal?> Values { get; } = [];
//}

//public sealed class FinancialFact
//{
//    public string Key { get; set; } = "";

//    public string Title { get; set; } = "";

//    public StatementLineType Type { get; set; }

//    public Dictionary<int, decimal?> Values { get; } = [];

//    public Dictionary<string, object> Metadata { get; } = [];
//}

//public sealed class FinancialStatement
//{
//    public FinancialStatementType Type { get; set; }

//    public List<FinancialPeriod> Periods { get; } = [];

//    public Dictionary<string, FinancialFact> Facts { get; } = [];
//}

public interface IFinancialMapper
{
    FinancialStatement Map(
        StatementDocument document);
}

public interface IMappingRule
{
    bool CanMap(
        StatementLine line);

    string Map(
        StatementLine line);
}


public sealed class StatementDocument
{
    public string Title { get; set; } = "";

    public StatementLayoutInfo Layout { get; set; }

    //public FinancialStatementType StatementType { get; set; }

    public string? PeriodTitle { get; set; }

    public List<StatementColumn> Columns { get; } = [];

    public List<StatementLine> Lines { get; } = [];
}


public sealed class StatementValue
{
    public int ColumnIndex { get; set; }

    public string? Value { get; set; }
}

public sealed class StatementLine
{
    public string Title { get; set; } = "";

    public StatementLineType Type { get; set; }

    public List<StatementValue> Values { get; } = [];
}

//public enum ValidationSeverity
//{
//    Info,
//    Warning,
//    Error
//}