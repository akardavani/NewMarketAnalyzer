namespace System.Linq
{
    public class QueryParam : Pagination
    {
        public bool GetTotals { get; set; }
        public FilterRule Filter { get; set; }
    }
}
