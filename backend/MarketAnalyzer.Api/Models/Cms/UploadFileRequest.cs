namespace MarketAnalyzer.Api.Models
{
    public class UploadFileRequest
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
        public required string Content { get; init; }
    }
}
