using System.Text.Json.Serialization;

public class TsetmcCodalStatementItem
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("sentDateTime_Gregorian")]
    public DateTime? SentDateTimeGregorian { get; set; }

    [JsonPropertyName("publishDateTime_Gregorian")]
    public DateTime? PublishDateTimeGregorian { get; set; }

    [JsonPropertyName("publishDateTime_DEven")]
    public long? PublishDateTimeDEven { get; set; }

    [JsonPropertyName("reportSubType")]
    public int? ReportSubType { get; set; }

    [JsonPropertyName("pageID")]
    public int? PageId { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }
}
