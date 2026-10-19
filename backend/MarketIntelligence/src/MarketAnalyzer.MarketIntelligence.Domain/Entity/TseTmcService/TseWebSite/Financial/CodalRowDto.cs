using Newtonsoft.Json;

namespace MarketAnalyzer.MarketIntelligence.Domain.Entity
{
    public class CodalRowDto
    {
        [JsonProperty("itemTitle")]
        public string ItemTitle { get; set; }

        // مقادیر در API معمولاً لیستی از اعداد برای دوره‌های مختلف هستند
        [JsonProperty("values")]
        public List<decimal?> Values { get; set; }
    }

}
