using System.Collections.Generic;

namespace System.Linq
{
    public interface IPagination
    {
        int Page { get; set; }
        int Rows { get; set; }
        IDictionary<string, SortType> OrderBy { get; set; }
    }
}
