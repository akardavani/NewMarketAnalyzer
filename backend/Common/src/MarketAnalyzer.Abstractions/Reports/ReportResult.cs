namespace MarketAnalyzer.Abstractions
{
    public class ReportResult
    {
        public required string ContentType { get; init; }
        public required string FileName { get; init; }
        public required byte[] Data { get; init; }
    }
}
