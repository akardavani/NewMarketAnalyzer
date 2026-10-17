using MarketIntelligence.Domain;

namespace MarketIntelligence.Application
{
    /// <summary>
    /// مسئول محاسبه متریک‌های قدرت نسبی (Relative Strength) نماد.
    /// این کلاس با مقایسه بازدهی سهم نسبت به شاخص کل و شاخص صنعت، 
    /// قدرت روند سهم را در بازه‌های زمانی مختلف کمی‌سازی می‌کند.
    /// </summary>
    public static class RelativeStrengthCalculator
    {
        /// <summary>
        /// محاسبه تفاوت بازدهی سهم با شاخص کل بازار.
        /// بازدهی مثبت نشان‌دهنده عملکرد بهتر (Outperformance) سهم نسبت به کلیت بازار است.
        /// </summary>
        /// <param name="stock">لیست کندل‌های سهم</param>
        /// <param name="market">لیست کندل‌های شاخص کل بازار</param>
        /// <param name="days">بازه زمانی مورد نظر برای محاسبه (مثلاً 30، 60، 90 روز)</param>
        public static decimal RelativeStrengthVsMarket(
            IReadOnlyList<MarketCandle> stock,
            IReadOnlyList<MarketCandle> market,
            int days)
        {
            var stockReturn =
                ReturnCalculator.CalculateReturn(stock, days);

            var marketReturn =
                ReturnCalculator.CalculateReturn(market, days);

            // چرا تفریق؟ (stockReturn - marketReturn)
            // وقتی بازدهی سهم ۱۵٪ و بازدهی بازار ۱۰٪ است، نتیجه ۵٪+ می‌شود. این یعنی سهم ۵ درصد “آلفا” (Alpha) نسبت به بازار تولید کرده است.
            // اگر نتیجه منفی باشد، یعنی سهم نسبت به بازار ضعیف‌تر عمل کرده
            return stockReturn - marketReturn;
        }

        /// <summary>
        /// محاسبه تفاوت بازدهی سهم با شاخص صنعت مربوطه.
        /// این متد کمک می‌کند تا تشخیص دهیم سهم در درون صنعت خود یک پیشرو (Leader) است یا خیر.
        /// </summary>
        /// <param name="stock">لیست کندل‌های سهم</param>
        /// <param name="industry">لیست کندل‌های شاخص صنعت</param>
        /// <param name="days">بازه زمانی مورد نظر برای محاسبه</param>
        public static decimal RelativeStrengthVsIndustry(
            IReadOnlyList<MarketCandle> stock,
            IReadOnlyList<MarketCandle> industry,
            int days)
        {
            var stockReturn =
                ReturnCalculator.CalculateReturn(stock, days);

            var industryReturn =
                ReturnCalculator.CalculateReturn(industry, days);

            return stockReturn - industryReturn;
        }

        /// <summary>
        /// محاسبه امتیاز نهایی قدرت نسبی با وزن‌دهی به بازه‌های زمانی مختلف.
        /// از آنجایی که روند‌های کوتاه‌مدت اهمیت بیشتری در سیگنال‌های فعلی دارند، وزن بیشتری (50%) به بازه 30 روزه داده شده است.
        /// 
        /// در بازارهای مالی (مثل بورس ایران)، Recency Bias یا “اثر تازگی” بسیار قوی است. یعنی قدرت سهم در ۳۰ روز اخیر، 
        /// راهنمای بهتری برای حرکت در هفته آینده است تا قدرت سهم در ۹۰ روز پیش. این مدلِ وزن‌دهی (50/30/20) دقیقاً همین رفتارِ “دنبال‌کننده روند” (Trend Following) را پیاده می‌کند
        /// </summary>
        /// <param name="rs30">قدرت نسبی 30 روزه</param>
        /// <param name="rs60">قدرت نسبی 60 روزه</param>
        /// <param name="rs90">قدرت نسبی 90 روزه</param>
        /// <returns>امتیاز نهایی قدرت نسبی که برای رتبه‌بندی (Ranking) استفاده می‌شود.</returns>
        public static decimal RelativeStrengthScore(
           decimal rs30,
           decimal rs60,
           decimal rs90)
        {
            return
                rs30 * 0.5m + // وزن 50 درصد برای کوتاه‌مدت
                rs60 * 0.3m + // وزن 30 درصد برای میان‌مدت
                rs90 * 0.2m;  // وزن 20 درصد برای بلندمدت
        }       
    }
}
