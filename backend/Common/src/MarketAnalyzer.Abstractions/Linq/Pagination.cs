using System.Collections.Generic;

namespace System.Linq
{
    public abstract class Pagination : IPagination
    {
        public int Page { get; set; } = 1;
        public int Rows { get; set; } = 100;

        private IDictionary<string, SortType> _orderBy;
        public IDictionary<string, SortType> OrderBy
        {
            get => _orderBy ??= new Dictionary<string, SortType>();
            set => _orderBy = value;
        }
    }
}
