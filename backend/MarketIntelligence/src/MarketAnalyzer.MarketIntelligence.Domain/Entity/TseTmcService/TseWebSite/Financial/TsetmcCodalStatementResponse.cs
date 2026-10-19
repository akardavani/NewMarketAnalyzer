using System.Text.Json.Serialization;

public class TsetmcCodalStatementResponse
{
    [JsonPropertyName("statemetnContent")]
    public List<TsetmcCodalStatementItem> StatemetnContent { get; set; } = new();
}
