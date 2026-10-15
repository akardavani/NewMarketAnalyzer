using System.Text.Json.Serialization;

namespace MarketAnalyzer.Sejam
{
    public class SejamResponse<TData>
    {
        public bool Successed => Error is null;

        public TData Data { get; set; }
        public SejamMeta Meta { get; set; }
        public SejamError Error { get; set; }

        [JsonIgnore]
        public string JsonText { get; set; }
    }
}