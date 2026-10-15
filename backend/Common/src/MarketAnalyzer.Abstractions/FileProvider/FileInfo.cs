namespace MarketAnalyzer.Abstractions
{
    public class FileInfo
    {
        /// <summary>
        /// نام فایل
        /// </summary>
        public required string FileName { get; init; }
        /// <summary>
        /// Mime نوع
        /// </summary>
        public required string MimeType { get; init; }
        /// <summary>
        /// محتوای فایل
        /// </summary>
        public required byte[] Content { get; init; }

        /// <summary>
        /// حجم فایل
        /// </summary>
        public long Length => Content?.LongLength ?? 0;
    }
}
