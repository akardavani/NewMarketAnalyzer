using System.Text.Json.Serialization;

namespace MarketAnalyzer.Domain
{
    public class InvestmentFundChart : IEntity
    {
        public string Id { get; set; }
        /// <summary>
        /// شماره ثبت نزد سازمان
        /// </summary>
        public string RegNo { get; set; }
        public string FundInstrumentId { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
        [JsonPropertyName("issueNav")]
        public decimal IssuenceNav { get; set; }
        [JsonPropertyName("cancelNav")]
        public decimal CancellationNav { get; set; }
        [JsonPropertyName("statisticalNav")]
        public decimal StatisticalNav { get; set; }
    }
}
