using System.Text.Json.Serialization;

namespace MarketAnalyzer.MarketIntelligence.Domain.Entity
{
    public class CodalStatementDto : IFinancialReport
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("sentDateTime_Gregorian")]
        public DateTime SentDateTimeGregorian { get; set; }

        [JsonPropertyName("publishDateTime_Gregorian")]
        public DateTime PublishDateTimeGregorian { get; set; }

        [JsonPropertyName("publishDateTime_DEven")]
        public int PublishDateTimeDEven { get; set; }

        [JsonPropertyName("reportSubType")]
        public int ReportSubType { get; set; }

        [JsonPropertyName("pageID")]
        public int PageId { get; set; }

        // متن خام گزارش مالی
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }


}
