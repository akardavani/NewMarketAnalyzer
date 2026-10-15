namespace MarketAnalyzer.UI.Web
{
    public class UiTable
    {
        public string Id { get; set; } = $"tbl_{Guid.NewGuid():N}";

        public List<string> Columns { get; set; } = new();

        public List<Dictionary<string, object?>> Rows { get; set; } = new();

        public bool EnableSearch { get; set; } = true;

        public bool EnablePaging { get; set; } = true;

        public bool EnableSorting { get; set; } = true;

        public int PageSize { get; set; } = 10;
    }
}
