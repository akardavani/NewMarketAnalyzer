using System;

namespace MarketAnalyzer.DataProvider.MabnaModels
{
    public class Metadata
    {
        public long Version { get; set; }

        public RecordState State { get; set; }

        public string Type { get; set; }

        public DateTime? InsertDateTime { get; set; }

        public DateTime? UpdateDateTime { get; set; }

        public DateTime? DeleteDateTime { get; set; }
    }
}
