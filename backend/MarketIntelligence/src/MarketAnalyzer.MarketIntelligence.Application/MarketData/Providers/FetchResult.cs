namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>
    /// نتیجهٔ تایپ‌شدهٔ یک دریافت از Provider. Providerها خطا را نمی‌بلعند و لیست خالی هم به‌جای
    /// شکست برنمی‌گردانند؛ همیشه یکی از چهار حالت <see cref="FetchStatus"/> را گزارش می‌کنند.
    /// </summary>
    /// <typeparam name="T">نوع داده‌ای که منبع تحویل می‌دهد.</typeparam>
    public sealed class FetchResult<T>
    {
        private FetchResult(FetchStatus status, T? data, int count, DateTime? sourceTimestampUtc, string? errorMessage, Exception? exception)
        {
            Status = status;
            Data = data;
            Count = count;
            SourceTimestampUtc = sourceTimestampUtc;
            ErrorMessage = errorMessage;
            Exception = exception;
        }

        /// <summary>وضعیت دریافت.</summary>
        public FetchStatus Status { get; }

        /// <summary>داده دریافت‌شده؛ در حالت Failed و SuccessEmpty خالی است.</summary>
        public T? Data { get; }

        /// <summary>تعداد رکوردهای دریافت‌شده (برای گزارش و اعتبارسنجی).</summary>
        public int Count { get; }

        /// <summary>زمان ثبت داده در منبع (UTC)، اگر منبع آن را بدهد.</summary>
        public DateTime? SourceTimestampUtc { get; }

        /// <summary>پیام خطا برای حالت‌های Failed و Partial.</summary>
        public string? ErrorMessage { get; }

        /// <summary>استثنای اصلی برای لاگ در حالت‌های Failed و Partial.</summary>
        public Exception? Exception { get; }

        /// <summary>آیا دریافت بدون خطا انجام شده است (Success یا SuccessEmpty)؟</summary>
        public bool IsCompleteResponse => Status is FetchStatus.Success or FetchStatus.SuccessEmpty;

        /// <summary>دریافت موفق با داده.</summary>
        public static FetchResult<T> Success(T data, int count, DateTime? sourceTimestampUtc = null)
            => new(FetchStatus.Success, data, count, sourceTimestampUtc, null, null);

        /// <summary>دریافت موفق بدون داده (پاسخ معتبر ولی خالی).</summary>
        public static FetchResult<T> SuccessEmpty(DateTime? sourceTimestampUtc = null)
            => new(FetchStatus.SuccessEmpty, default, 0, sourceTimestampUtc, null, null);

        /// <summary>دریافت ناقص: بخشی از داده معتبر است.</summary>
        public static FetchResult<T> Partial(T data, int count, string errorMessage, Exception? exception = null, DateTime? sourceTimestampUtc = null)
            => new(FetchStatus.Partial, data, count, sourceTimestampUtc, errorMessage, exception);

        /// <summary>دریافت ناموفق.</summary>
        public static FetchResult<T> Failed(string errorMessage, Exception? exception = null)
            => new(FetchStatus.Failed, default, 0, null, errorMessage, exception);
    }
}
