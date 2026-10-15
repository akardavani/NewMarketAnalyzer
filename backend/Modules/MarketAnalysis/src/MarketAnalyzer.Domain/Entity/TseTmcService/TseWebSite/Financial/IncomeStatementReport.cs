public class IncomeStatementReport
{
    public string Title { get; set; }
    public DateTime? PublishDateTimeGregorian { get; set; }
    public long? PublishDateTimeDEven { get; set; }

    public string FiscalPeriodEnd { get; set; }
    public string ReportLengthMonths { get; set; }
    public string AuditStatus { get; set; }

    public List<IncomeStatementColumn> Columns { get; set; } = new();
    public List<IncomeStatementRow> Rows { get; set; } = new();

    public IncomeStatementKeyMetrics Metrics { get; set; } = new();
}
