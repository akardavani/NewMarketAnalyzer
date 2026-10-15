namespace MarketAnalyzer.Abstractions
{
    public class WriteFileRequest
    {
        /// <summary>
        /// موضوع فایل
        /// </summary>
        public required int Purpose { get; init; }
        /// <summary>
        /// نام فایل
        /// </summary>
        public required string FileName { get; init; }
        /// <summary>
        /// محتوای فایل
        /// </summary>
        public required byte[] Content { get; init; }
        /// <summary>
        /// شناسه اپراتور
        /// </summary>
        public required int OperatorId { get; init; }
    }
}
