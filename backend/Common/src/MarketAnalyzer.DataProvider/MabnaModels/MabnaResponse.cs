using System.Collections.Generic;

namespace MarketAnalyzer.DataProvider.MabnaModels
{
    public class MabnaResponse<T> where T : EntityBase
    {
        public ICollection<T> Data { get; set; }

        public ResponseError Error { get; set; }
    }
}
