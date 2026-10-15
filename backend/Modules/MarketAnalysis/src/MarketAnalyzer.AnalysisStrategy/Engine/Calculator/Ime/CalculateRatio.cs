using MarketAnalyzer.Domain;
using MarketAnalyzer.Analysis.Infrastructure;

namespace MarketAnalyzer.AnalysisStrategy
{
    public static class CalculateRatio
    {
        /// <summary>
        /// سنجش رابطه بین دو دارایی 
        /// به ما کمک می‌کند تا وضعیت و رفتار نسبی دو دارایی را مورد تحلیل قرار دهیم
        /// 
        /// قیمت سهم "الف" را بر قیمت سهم "ب" تقسیم می‌کنید
        /// تجزیه و تحلیل عملکرد:
        /// با مشاهده تغییرات این نسبت در طول زمان، می‌توان به تحلیل روندهای بالقوه در بازار پرداخت.
        /// مثلاً اگر نسبت در حال افزایش باشد، نشان‌دهنده‌ی آن است که سهم "الف" نسبت به سهم "ب" بهتر عمل می‌کند 
        /// </summary>
        /// <param name="targetCandle">نمادی که نسبت به نماد پایه بررسی میشود</param>
        /// <param name="baseCandle">نماد پایه</param>
        /// <param name="asPercent">آیا درصدی باشد</param>
        /// <returns></returns>
        public static List<CandleResult> Calculate(List<MarketCandle> targetCandle, List<MarketCandle> baseCandle, bool asPercent)
        {
            // ساخت نقشه‌های تاریخ → مقدار
            var futuresMap = targetCandle.ToDictionary(c => c.Date.ToDateOnly(), c => c.Close);
            var spotsMap = baseCandle.ToDictionary(c => c.Date.ToDateOnly(), c => c.Close);

            // تاریخ‌های مشترک مرتب شده
            var commonDates = futuresMap.Keys.Intersect(spotsMap.Keys).OrderBy(d => d).ToList();

            // داده‌های ratio بر اساس تاریخ‌های مشترک
            var ratioPairs = new List<CandleResult>();

            foreach (var date in commonDates)
            {
                var fv = futuresMap[date];
                if (fv == 0)
                {
                    continue;
                }

                var sv = spotsMap[date];
                decimal ratio = sv != 0 ? fv / sv : 0; // اگر sv برابر صفر بود مقدار را به 0 بدهیم
                //if (asPercent)
                //{
                //    ratio = (ratio - 1) * 100; // به درصد: (Futures/Spot - 1) * 100
                //}
                if (asPercent)
                {
                    ratio = ratio * 100; // به درصد: (Futures/Spot - 1) * 100
                }

                ratioPairs.Add(new CandleResult
                {
                    Date = date,
                    PersianDate = DateTimeExtensions.ToPersian(date),
                    Growth = ratio
                });
            }

            return ratioPairs;
        }
    }
}
