namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// اطلاعات اضافهٔ نمادهای بورس که در ستون MetadataJson ذخیره می‌شود (نسخهٔ ساختار: <see cref="CurrentVersion"/>).
    /// با تغییر ساختار، نسخه را بالا ببر تا مصرف‌کننده‌ها بدانند چه چیزی بخوانند.
    /// </summary>
    public sealed class TsetmcStockMetadata
    {
        /// <summary>نسخهٔ فعلی ساختار.</summary>
        public const int CurrentVersion = 1;

        /// <summary>نام کامل نماد (مثل «فولاد مبارکه اصفهان»).</summary>
        public string? FullName { get; set; }

        /// <summary>نام شرکت.</summary>
        public string? CompanyName { get; set; }

        /// <summary>نماد لاتین کوتاه (Mnemonic).</summary>
        public string? Mnemonic { get; set; }

        /// <summary>کد چهار رقمی شرکت.</summary>
        public string? FourDigitCompanyCode { get; set; }

        /// <summary>شناسهٔ بین‌المللی (ISIN) شرکت.</summary>
        public string? CIsin { get; set; }

        /// <summary>کد گروه ابزار مالی.</summary>
        public string? InstrumentGroupId { get; set; }

        /// <summary>بخش بازار.</summary>
        public string? MarketSegment { get; set; }

        /// <summary>کد تابلو.</summary>
        public long? BoardCode { get; set; }

        /// <summary>کد صنعت.</summary>
        public string? SectorCode { get; set; }

        /// <summary>کد زیرصنعت.</summary>
        public string? SubSectorCode { get; set; }

        /// <summary>نوع ابزار (YVal).</summary>
        public long? InstrumentType { get; set; }

        /// <summary>تعداد سهام یا اوراق.</summary>
        public long? SharesOrBondsNumber { get; set; }

        /// <summary>جریان بازار (۱ بورس، ۲ فرابورس، ۴ پایه، ۷ کالا...).</summary>
        public int? Flow { get; set; }

        /// <summary>وضعیت اعتبار نماد در منبع.</summary>
        public int? Valid { get; set; }
    }
}
