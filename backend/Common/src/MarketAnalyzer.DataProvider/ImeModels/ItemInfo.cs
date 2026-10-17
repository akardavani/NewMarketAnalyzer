using System.Collections.Generic;

namespace MarketAnalyzer.DataProvider.ImeModels
{
    public class ItemInfo<T>
    {
        public T Item { get; set; }
        public List<Link> Links { get; set; }
    }
}
