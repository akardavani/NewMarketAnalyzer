public class IncomeStatementRow
{
    public string ItemTitle { get; set; }
    public List<decimal?> Values { get; set; } = new();
}
