
using MarketAnalyzer.Analysis.Infrastructure;

namespace MarketAnalyzer.AnalysisStrategy
{
    public static class CalculateYTM
    {
        public static List<CandleResult> Calculate(
            List<YtmCandle> futures,
            List<YtmCandle> spots,
            bool asPercent)
        {
            var futuresMap = futures
                .GroupBy(c => c.Date.ToDateOnly())
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(x => x.Date).Last());

            var spotsMap = spots
                .GroupBy(c => c.Date.ToDateOnly())
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(x => x.Date).Last());

            var commonDates = futuresMap.Keys
                .Intersect(spotsMap.Keys)
                .OrderBy(d => d)
                .ToList();

            var result = new List<CandleResult>();

            foreach (var date in commonDates)
            {
                var future = futuresMap[date];
                var spot = spotsMap[date];

                var fv = future.Close;
                var sv = spot.Close;

                var maturityDate = future.MaturityDate;

                double yearsToMaturity =
                    maturityDate > 0
                    ? maturityDate / 365.0
                    : 1.0;

                double ytm =
                    sv != 0
                    ? Math.Pow((double)fv / (double)sv, 1.0 / yearsToMaturity) - 1
                    : 0;

                if (asPercent)
                    ytm *= 100;

                if (ytm > 300 || ytm < -20)
                    continue;

                result.Add(new CandleResult
                {
                    Date = date,
                    PersianDate = DateTimeExtensions.ToPersian(date),
                    Growth = (decimal)ytm
                });
            }

            return result;
        }
    }
}

//using MarketAnalyzer.SharedKernel;

//namespace MarketAnalyzer.AnalysisStrategy
//{
//    public static class CalculateYTM
//    {
//        public static List<CandleResult> Calculate(List<YtmCandle> futures, List<YtmCandle> spots, bool asPercent)
//        {
//            var futuresMap = futures.ToDictionary(c => c.Date.ToDateOnly(), c => c.Close);
//            var spotsMap = spots.ToDictionary(c => c.Date.ToDateOnly(), c => c.Close);

//            // تاریخ‌های مشترک مرتب شده
//            var commonDates = futuresMap.Keys.Intersect(spotsMap.Keys).OrderBy(d => d).ToList();

//            // محاسبه YTM بر اساس تاریخ‌های مشترک
//            var ytmPairs = new List<CandleResult>();
//            foreach (var date in commonDates)
//            {
//                var fv = futuresMap[date];
//                var sv = spotsMap[date];

//                var maturityDate = futures.FirstOrDefault(e => e.Date.ToDateOnly() == date).MaturityDate;
//                double yearsToMaturity = maturityDate > 0 ? maturityDate / 365.0 : 1.0;

//                double ytm = sv != 0 ? Math.Pow((double)fv / (double)sv, 1.0 / yearsToMaturity) - 1 : 0;
//                if (asPercent)
//                {
//                    ytm *= 100;
//                }

//                // حذف اعداد پرت
//                if (ytm > 300 || ytm < -20)
//                {
//                    continue;
//                }

//                ytmPairs.Add(new CandleResult
//                {
//                    Date = date,
//                    PersianDate = DateTimeExtensions.ToPersian(date),
//                    Growth = (decimal)ytm
//                });
//            }

//            return ytmPairs;
//        }
//    }
//}
