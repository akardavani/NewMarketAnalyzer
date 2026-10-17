namespace MarketIntelligence.Application
{
    public static class RankingEngine
    {
        // ۱. رتبه‌بندی کل بازار (Cross-Sectional Ranking)
        public static List<IranEquityStrategyResult> RankUniverse(List<IranEquityStrategyResult> results)
        {
            if (!results.Any()) return results;

            // الف) محاسبه Z-Score برای نرمال‌سازی فاکتورها (نسبت به میانگین بازار)
            var avgMom = results.Average(x => x.RawMomentum);
            var stdMom = CalculateStdDev(results.Select(x => x.RawMomentum));

            var avgLiq = results.Average(x => x.RawLiquidity);
            var stdLiq = CalculateStdDev(results.Select(x => x.RawLiquidity));

            var avgQuality = results.Average(x => x.RawQuality);
            var stdQuality = CalculateStdDev(results.Select(x => x.RawQuality));

            foreach (var res in results)
            {
                var zQuality = stdQuality == 0 ? 0 : (res.RawQuality - avgQuality) / stdQuality;

                // ترکیب وزن‌دار: ۴۰٪ مومنتوم، ۴۰٪ نقدینگی، ۲۰٪ کیفیت صنعت
                var zMom = stdMom == 0 ? 0 : (res.RawMomentum - avgMom) / stdMom;
                var zLiq = stdLiq == 0 ? 0 : (res.RawLiquidity - avgLiq) / stdLiq;

                res.FinalScore = 
                    (zMom * 0.40m) + 
                    (zLiq * 0.40m) + 
                    (zQuality * 0.20m);

                // پر کردن دیکشنری برای نمایش در خروجی
                res.FactorScores["Momentum"] = Math.Round(zMom, 2);
                res.FactorScores["Liquidity"] = Math.Round(zLiq, 2);
                res.FactorScores["Quality"] = Math.Round(zQuality, 2);
            }

            // ب) مرتب‌سازی و تعیین رتبه درصدی (Percentile)
            var ranked = results.OrderByDescending(x => x.FinalScore).ToList();
            for (int i = 0; i < ranked.Count; i++)
            {
                ranked[i].Percentile = (decimal)(ranked.Count - i) / ranked.Count;
                ranked[i].Tier = DetermineTier(ranked[i].Percentile);
                ranked[i].IsCandidate = ranked[i].Percentile >= 0.85m; // ۱۵ درصد برتر
            }

            return ranked;
        }

        // ۲. تحلیل صنایع (Sector Analysis)
        public static List<SectorAnalysisResult> AnalyzeSectors(
            List<IranEquityStrategyResult> rankedStocks,
            List<StockAnalysisInput> universe)
        {
            // مپ کردن نام صنعت به نمادها
            var sectorGroups = universe.GroupBy(u => u.Data.IndustryName);
            var report = new List<SectorAnalysisResult>();

            foreach (var group in sectorGroups)
            {
                var sectorName = group.Key;
                var symbolNames = group.Select(g => g.Data.ExcelData.Symbol).ToList();

                // استخراج نتایج رتبه‌بندی شده برای نمادهای این صنعت
                var stocksInSector = rankedStocks.Where(s => symbolNames.Contains(s.Symbol)).ToList();

                if (!stocksInSector.Any()) continue;

                var avgScore = stocksInSector.Average(s => s.FinalScore);
                var topTierCount = stocksInSector.Count(s => s.Tier.StartsWith("A"));
                var moneyFlow = stocksInSector.Sum(s => s.RawLiquidity);

                report.Add(new SectorAnalysisResult
                {
                    SectorName = sectorName,
                    AverageScore = avgScore,
                    TopTierCount = topTierCount,
                    TotalMoneyFlow = moneyFlow,
                    SymbolCount = stocksInSector.Count,
                    Status = DetermineSectorStatus(avgScore, topTierCount, moneyFlow)
                });
            }

            return report.OrderByDescending(x => x.AverageScore).ToList();
        }

        // متدهای کمکی (Helper Methods)
        private static string DetermineTier(decimal percentile)
        {
            if (percentile >= 0.95m) return "A+";
            if (percentile >= 0.85m) return "A";
            if (percentile >= 0.70m) return "B";
            return "C/D";
        }

        private static string DetermineSectorStatus(decimal avgScore, int topCount, decimal moneyFlow)
        {
            if (avgScore > 0.5m && topCount >= 2) return "🔥 Hot Sector (Leading)";
            if (moneyFlow > 0) return "✅ Accumulating";
            if (avgScore < -0.5m) return "❄️ Cold (Lagging)";
            return "Neutral";
        }

        private static decimal CalculateStdDev(IEnumerable<decimal> values)
        {
            if (!values.Any()) return 0;
            double avg = (double)values.Average();
            double sum = values.Sum(d => Math.Pow((double)d - avg, 2));
            return (decimal)Math.Sqrt(sum / values.Count());
        }
    }
}