using Newtonsoft.Json;

namespace MarketIntelligence.Domain
{
    public class Etf
    {
        [JsonProperty("etf")]
        public TseTmcETFNavInfo TseTmcETFNavInfo { get; set; }
    }

    public class TseTmcETFNavInfo 
    {
        [JsonProperty("insCode")]
        public long InsCode { get; set; }

        [JsonProperty("deven")]
        public long Deven { get; set; }

        [JsonProperty("hEven")]
        public long HEven { get; set; }

        [JsonProperty("pRedTran")]
        public decimal CancellationNAV { get; set; }

        [JsonProperty("pSubTran")]
        public decimal PSubTran { get; set; }

        [JsonProperty("iClose")]
        public int IClose { get; set; }
    }
}
