namespace MarketAnalyzer.AnalysisStrategy
{
    public sealed class BottomReversalResult : IranEquityStrategyResult
    {
        /// <summary>
        /// امتیاز خام اشباع فروش.
        /// هرچه RSI و MFI پایین‌تر باشند، این عدد بیشتر می‌شود.
        /// </summary>
        public decimal RawOversold { get; set; }

        /// <summary>
        /// امتیاز خام نزدیکی به کف.
        /// هرچه سهم به کف ۶۰ یا ۱۲۰ روزه نزدیک‌تر باشد، این عدد بیشتر است.
        /// </summary>
        public decimal RawProximity { get; set; }

        /// <summary>
        /// قدرت برگشت.
        /// ترکیبی از کندل برگشتی، حجم غیرعادی و برگشت قیمت.
        /// </summary>
        public decimal RawReversalStrength { get; set; }

        /// <summary>
        /// کیفیت زمینه‌ای سهم یا صنعت.
        /// مثلا SectorRotationScore، ZScoreIndustry یا قدرت نسبی صنعت.
        /// </summary>
        public decimal RawBottomQuality { get; set; }

        /// <summary>
        /// فاکتورهای خام مخصوص استراتژی برگشت از کف.
        /// برای شفافیت خروجی و دیباگ.
        /// </summary>
        public Dictionary<string, decimal> RawFactors { get; set; } = new();

        /// <summary>
        /// متریک‌های قابل نمایش برای خروجی.
        /// </summary>
        public Dictionary<string, decimal> Metrics { get; set; } = new();

        /// <summary>
        /// توضیح تحلیلی برای اینکه چرا این سهم انتخاب شده.
        /// </summary>
        public List<string> Reasons { get; set; } = new();
    }

}
