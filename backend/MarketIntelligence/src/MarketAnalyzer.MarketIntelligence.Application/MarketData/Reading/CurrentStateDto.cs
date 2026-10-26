namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Reading
{
    /// <summary>آخرین وضعیت یک نماد که از قرارداد خواندن بیرون می‌آید.</summary>
    public sealed record CurrentStateDto
    {
        /// <summary>ارجاع به نماد.</summary>
        public required SymbolRef Symbol { get; init; }

        /// <summary>آخرین قیمت.</summary>
        public decimal? LastPrice { get; init; }

        /// <summary>قیمت پایانی.</summary>
        public decimal? ClosePrice { get; init; }

        /// <summary>قیمت پایانی روز قبل.</summary>
        public decimal? PreviousClosePrice { get; init; }

        /// <summary>حداقل قیمت مجاز.</summary>
        public decimal? PriceLimitMin { get; init; }

        /// <summary>حداکثر قیمت مجاز.</summary>
        public decimal? PriceLimitMax { get; init; }

        /// <summary>حجم امروز.</summary>
        public decimal? Volume { get; init; }

        /// <summary>ارزش امروز.</summary>
        public decimal? Value { get; init; }

        /// <summary>زمان ثبت در منبع (UTC).</summary>
        public required DateTime SourceTimestampUtc { get; init; }
    }
}
