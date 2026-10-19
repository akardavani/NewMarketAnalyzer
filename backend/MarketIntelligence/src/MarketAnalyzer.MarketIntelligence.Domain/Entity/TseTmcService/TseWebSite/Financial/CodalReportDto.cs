using System.Text.Json.Serialization;

namespace MarketAnalyzer.MarketIntelligence.Domain.Entity
{

    // این مدل دقیقاً منطبق بر JSON ارسالی شماست
    public class CodalReportDto
    {
        [JsonPropertyName("statemetnContent")]
        public List<CodalStatementDto> Statements { get; set; } = new();
    }


}
