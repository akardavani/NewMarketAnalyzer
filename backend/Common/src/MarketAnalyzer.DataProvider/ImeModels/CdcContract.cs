using System.Text.Json.Serialization;

namespace MarketAnalyzer.DataProvider.ImeModels
{
    /// <summary>
    /// اطلاعات قرارداد گواهی سپرده
    /// </summary>
    public class CdcContract
    {
        public int Id { get; set; }
        [JsonPropertyName("contractID")]
        public int ContractId { get; set; }
        [JsonPropertyName("commodityID")]
        public int CommodityId { get; set; }
        public string CommodityName { get; set; }
        public string Currency { get; set; }
        public string Symbol { get; set; }
        public int Lot { get; set; }
        public decimal ConversionRate { get; set; }
        public bool Active { get; set; }
    }
}
