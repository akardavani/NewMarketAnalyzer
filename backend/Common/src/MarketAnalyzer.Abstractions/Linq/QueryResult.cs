using System.Collections.Generic;

namespace System.Linq
{
    public class QueryResult<T>
    {
        private List<T> _rows;
        public List<T> Rows
        {
            get => _rows ??= [];
            set => _rows = value;
        }

        public long ItemsCount { get; set; }
    }
}
