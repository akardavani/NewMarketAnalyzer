namespace MarketAnalyzer.Abstractions
{
    public class ReadFileRequest
    {
        /// <summary>
        /// شناسه فایل
        /// </summary>
        public required string FileId { get; init; }
        /// <summary>
        /// شناسه اپراتور
        /// </summary>
        public required int OperatorId { get; init; }
    }
}
