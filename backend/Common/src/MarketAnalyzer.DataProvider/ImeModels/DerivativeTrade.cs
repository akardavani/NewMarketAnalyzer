using System;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.DataProvider.ImeModels
{
    /// <summary>
    /// معاملات مشتقه
    /// </summary>
    public class DerivativeTrade
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string ContractCode { get; set; }
        public string ContractName { get; set; }
        public string ContractDescription { get; set; }
        public int BrokerId { get; set; }
        public string BrokerName { get; set; }
        public int CustomerId { get; set; }
        [JsonPropertyName("customerNationalID")]
        public string CustomerNationalId { get; set; }
        public string CustomerName { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime Time { get; set; }
        public string DetailedDescription { get; set; }
    }
}
