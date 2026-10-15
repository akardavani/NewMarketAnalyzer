using System.Text.Json.Serialization;

namespace MarketAnalyzer.DataProvider.FipiranModels
{
    public class FundType
    {
        [JsonPropertyName("fundType")]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
