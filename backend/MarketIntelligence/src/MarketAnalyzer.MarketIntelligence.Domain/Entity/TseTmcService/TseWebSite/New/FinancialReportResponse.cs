using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;


//public class FinancialReportResponse
//{
//    [JsonPropertyName("statemetnContent")]
//    public List<FinancialReportItem> StatemetnContent { get; set; } = [];
//}

//public class FinancialReportItem
//{
//    [JsonPropertyName("title")]
//    public string? Title { get; set; }

//    [JsonPropertyName("publishDateTime_Gregorian")]
//    public DateTime PublishDateTime_Gregorian { get; set; }

//    [JsonPropertyName("reportSubType")]
//    public int ReportSubType { get; set; }

//    [JsonPropertyName("pageID")]
//    public int PageID { get; set; }

//    [JsonPropertyName("content")]
//    public string? Content { get; set; }
//}

//public class FinancialTable
//{
//    public List<string> Headers { get; set; } = [];

//    public List<FinancialRow> Rows { get; set; } = [];
//}

//public class FinancialRow
//{
//    public List<string> Cells { get; set; } = [];
//}

//public class FinancialStatement
//{
//    public string Title { get; set; } = "";

//    public List<FinancialColumn> Columns { get; set; } = [];

//    public List<FinancialItem> Items { get; set; } = [];
//}

//public class FinancialColumn
//{
//    public int Index { get; set; }

//    public string Header { get; set; } = "";
//}

//public class FinancialItem
//{
//    public string Name { get; set; } = "";

//    public Dictionary<int, string> Values { get; set; } = [];
//}