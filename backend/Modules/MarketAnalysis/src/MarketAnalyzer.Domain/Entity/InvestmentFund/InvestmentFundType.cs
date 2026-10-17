using System.Text.Json.Serialization;

namespace MarketAnalyzer.Domain
{
    public class InvestmentFundType: IEntity
    {
        public string Id { get; set; }
        [JsonPropertyName("fundType")] 
        public int Type { get; set; }
        [JsonPropertyName("name")] 
        public string Name { get; set; }
        [JsonPropertyName("isActive")] 
        public bool IsActive { get; set; }
    }
}
