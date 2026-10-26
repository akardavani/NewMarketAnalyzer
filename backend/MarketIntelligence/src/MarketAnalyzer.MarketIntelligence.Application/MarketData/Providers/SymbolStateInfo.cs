namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>آخرین قیمت و وضعیت یک نماد آن‌طور که Provider گزارش می‌کند.</summary>
    public sealed class SymbolStateInfo
    {
        /// <summary>کلید استاندارد نماد.</summary>
        public required string SymbolKey { get; init; }

        /// <summary>آخرین قیمت معامله.</summary>
        public decimal? LastPrice { get; init; }

        /// <summary>قیمت پایانی امروز.</summary>
        public decimal? ClosePrice { get; init; }

        /// <summary>قیمت اولین معامله.</summary>
        public decimal? OpenPrice { get; init; }

        /// <summary>بالاترین قیمت امروز.</summary>
        public decimal? HighPrice { get; init; }

        /// <summary>پایین‌ترین قیمت امروز.</summary>
        public decimal? LowPrice { get; init; }

        /// <summary>قیمت پایانی روز قبل.</summary>
        public decimal? PreviousClosePrice { get; init; }

        /// <summary>حداقل قیمت مجاز امروز.</summary>
        public decimal? PriceLimitMin { get; init; }

        /// <summary>حداکثر قیمت مجاز امروز.</summary>
        public decimal? PriceLimitMax { get; init; }

        /// <summary>حجم معاملات امروز.</summary>
        public decimal? Volume { get; init; }

        /// <summary>ارزش معاملات امروز.</summary>
        public decimal? Value { get; init; }

        /// <summary>تعداد معاملات امروز.</summary>
        public long? TradeCount { get; init; }

        /// <summary>زمان ثبت وضعیت در منبع (UTC).</summary>
        public required DateTime SourceTimestampUtc { get; init; }
    }
}
