namespace MarketAnalyzer.SharedKernel
{
    public class UserAlertResult
    {
        public required long Id { get; init; }
        /// <summary>
        /// عنوان
        /// </summary>
        public required string Title { get; init; }
        /// <summary>
        /// پیغام
        /// </summary>
        public required string Message { get; init; }
    }
}
