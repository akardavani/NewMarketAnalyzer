namespace MarketIntelligence.Application
{
    public static class MetricBuilderEngine
    {
        //public static MarketDerivedMetrics Build(EnrichedStockData stock)
        //{   
        //    var dailyCandles = CalculationUtils.SortAscending(stock.DailyCandles);

        //    var latest = CalculationUtils.Latest(dailyCandles);
        //    var previous = CalculationUtils.Previous(dailyCandles);

        //    var lastPrice = stock.ExcelData?.LastPrice > 0
        //        ? stock.ExcelData.LastPrice
        //        : latest?.Close ?? 0;

        //    var closingPrice = latest?.Close ?? stock.ExcelData.ClosingPrice;

        //    var sma20 = MovingAverageCalculator.SMA(dailyCandles, 20);
        //    var sma50 = MovingAverageCalculator.SMA(dailyCandles, 50);
        //    var sma200 = MovingAverageCalculator.SMA(dailyCandles, 200);

        //    var avgVolume20 = VolumeCalculator.AverageVolume(dailyCandles, 20);
        //    var avgVolume30 = VolumeCalculator.AverageVolume(dailyCandles, 30);
        //    var avgVolume90 = VolumeCalculator.AverageVolume(dailyCandles, 90);

        //    var volumeToday = latest?.Volume ?? stock.ExcelData.VolumeToday;

        //    var atr14 = RiskCalculator.ATR(dailyCandles, 14);

        //    // پیدا کردن سطوح حمایتی و مقاومتی از کندل‌های ماهانه
        //    var support = SupportResistanceCalculator.FindMonthlySupport(stock.MonthlyCandles);
        //    var resistance = SupportResistanceCalculator.FindMonthlyResistance(stock.MonthlyCandles);

        //    var sorted = CalculationUtils.SortAscending(stock.DailyCandles);

        //    var rsiList = MomentumCalculator.CalculateRsiList(sorted, 14);
        //    var macdData = MomentumCalculator.CalculateMacd(sorted, 12, 26, 9);
        //    var stochList = MomentumCalculator.CalculateStochastic(sorted, 14);
        //    var cciList = MomentumCalculator.CalculateCci(sorted, 20);
        //    var momList = MomentumCalculator.CalculateMomentum(sorted, 10);
        //    var obvList = MomentumCalculator.CalculateObv(sorted);
        //    var vwmacdList = MomentumCalculator.CalculateVwMacd(sorted, 12, 26);
        //    var cmfList = MomentumCalculator.CalculateCmf(sorted, 20);
        //    var mfiList = MomentumCalculator.CalculateMfiList(sorted, 14);

        //    return new MarketDerivedMetrics
        //    {
        //        LastPrice = lastPrice,
        //        ClosingPrice = closingPrice,
        //        FirstPrice = latest?.Open ?? stock.ExcelData.FirstPrice,
        //        High = latest?.High ?? stock.ExcelData.High,
        //        Low = latest?.Low ?? stock.ExcelData.Low,

        //        LastPriceChangePercent = CalculationUtils.ChangePercent(lastPrice, previous?.Close ?? 0),
        //        ClosingPriceChangePercent = CalculationUtils.ChangePercent(closingPrice, previous?.Close ?? 0),

        //        Return1Month = ReturnCalculator.Return(dailyCandles, 21),
        //        Return3Month = ReturnCalculator.Return(dailyCandles, 63),
        //        Return6Month = ReturnCalculator.Return(dailyCandles, 126),
        //        Return1Year = ReturnCalculator.Return(dailyCandles, 252),

        //        SMA20 = sma20,
        //        SMA50 = sma50,
        //        SMA200 = sma200,

        //        DistanceToSMA20 = CalculationUtils.DistancePercent(lastPrice, sma20),
        //        DistanceToSMA50 = CalculationUtils.DistancePercent(lastPrice, sma50),
        //        DistanceToSMA200 = CalculationUtils.DistancePercent(lastPrice, sma200),

        //        RSI14 = MomentumCalculator.RSI(dailyCandles, 14),
        //        MFI14 = MomentumCalculator.MFI(dailyCandles, 14),

        //        VolumeToday = volumeToday,
        //        AvgVolume20 = avgVolume20,
        //        AvgVolume30 = avgVolume30,
        //        AvgVolume90 = avgVolume90,

        //        VolumeRatio20 = CalculationUtils.SafeDivide(volumeToday, avgVolume20),
        //        VolumeRatio30 = CalculationUtils.SafeDivide(volumeToday, avgVolume30),
        //        VolumeRatio90 = CalculationUtils.SafeDivide(volumeToday, avgVolume90),

        //        VolumeExpansionScore = VolumeCalculator.VolumeExpansionScore(dailyCandles),


        //        // متریک‌های تابلویی (بر اساس Excel)

        //        DemandToTradeRatio = CalculateDemandToTradeRatio(stock.ExcelData),
        //        QueuePower = CalculateQueuePower(stock.ExcelData),
        //        RealMoneyFlow = CalculateRealMoneyFlow(stock.ExcelData),
        //        AccumulationScore = CalculateAccumulationScore(stock.ExcelData),

        //        // متریک‌های تکنیکال و فواصل
        //        DistanceToSupport = SupportResistanceCalculator.DistanceToSupport(lastPrice, support),
        //        DistanceToResistance = SupportResistanceCalculator.DistanceToResistance(lastPrice, resistance),

        //        DistanceTo52WeekHigh = FiftyTwoWeekCalculator.DistanceTo52WeekHigh(stock.DailyCandles),
        //        DistanceTo52WeekLow = FiftyTwoWeekCalculator.DistanceTo52WeekLow(stock.DailyCandles),

        //        DistanceTo20DayLow = PricePositionCalculator.DistanceToLowestLow(dailyCandles, 20, lastPrice),
        //        DistanceTo60DayLow = PricePositionCalculator.DistanceToLowestLow(dailyCandles, 60, lastPrice),
        //        DistanceTo120DayLow = PricePositionCalculator.DistanceToLowestLow(dailyCandles, 120, lastPrice),

        //        DistanceTo20DayHigh = PricePositionCalculator.DistanceToHighestHigh(dailyCandles, 20, lastPrice),
        //        DistanceTo60DayHigh = PricePositionCalculator.DistanceToHighestHigh(dailyCandles, 60, lastPrice),
        //        DistanceTo120DayHigh = PricePositionCalculator.DistanceToHighestHigh(dailyCandles, 120, lastPrice),

        //        ATR14 = atr14,
        //        ATR14Percent = lastPrice > 0 ? atr14 / lastPrice * 100 : 0,
        //        Volatility30 = TrendCalculator.Volatility(stock.DailyCandles),
        //        SharpeLike = TrendCalculator.SharpeLike(stock.DailyCandles),
        //        BetaToMarket = RiskCalculator.Beta(dailyCandles, stock.MarketIndexCandles, 90),

        //        TrendStrength = TrendCalculator.CalculateTrendStrength(sma20, sma50, lastPrice),

        //        IsBullishCandle = CandlePatternCalculator.IsBullish(latest),
        //        IsCloseNearHigh = CandlePatternCalculator.IsCloseNearHigh(latest),
        //        LowerShadowRatio = CandlePatternCalculator.LowerShadowRatio(latest),
        //        BodyToRangeRatio = CandlePatternCalculator.BodyToRangeRatio(latest),
        //        ReversalCandleScore = CandlePatternCalculator.ReversalCandleScore(stock.DailyCandles, 3),

        //        // قدرت نسبی (RS) - ترکیب وزن‌دار برای رسیدن به یک عدد واحد
        //        RelativeStrengthPercentile = RelativeStrengthCalculator.RelativeStrengthScore(
        //            RelativeStrengthCalculator.RelativeStrengthVsMarket(stock.DailyCandles, stock.MarketIndexCandles, 30),
        //            RelativeStrengthCalculator.RelativeStrengthVsMarket(stock.DailyCandles, stock.MarketIndexCandles, 60),
        //            RelativeStrengthCalculator.RelativeStrengthVsMarket(stock.DailyCandles, stock.MarketIndexCandles, 90)
        //        ),

        //        // بازه‌های مختلف RS برای تحلیل دقیق‌تر
        //        RelativeStrength30Market = RelativeStrengthCalculator.RelativeStrengthVsMarket(stock.DailyCandles, stock.MarketIndexCandles, 30),
        //        RelativeStrength60Market = RelativeStrengthCalculator.RelativeStrengthVsMarket(stock.DailyCandles, stock.MarketIndexCandles, 60),
        //        RelativeStrength90Market = RelativeStrengthCalculator.RelativeStrengthVsMarket(stock.DailyCandles, stock.MarketIndexCandles, 90),


        //        RelativeStrength30Industry = RelativeStrengthCalculator.RelativeStrengthVsMarket(dailyCandles, stock.IndustryIndexCandles, 30),
        //        RelativeStrength60Industry = RelativeStrengthCalculator.RelativeStrengthVsMarket(dailyCandles, stock.IndustryIndexCandles, 60),
        //        RelativeStrength90Industry = RelativeStrengthCalculator.RelativeStrengthVsMarket(dailyCandles, stock.IndustryIndexCandles, 90),


        //        // چرخش صنعت و Z-Score
        //        SectorRotationScore = SectorRotationCalculator.Score(stock.IndustryIndexCandles),
        //        ZScoreIndustry = ZScoreCalculator.StockVsIndustryZScore(stock.DailyCandles, stock.IndustryIndexCandles),

        //        // ارزش‌گذاری
        //        ValueScore = ZScoreCalculator.CalculateValueScore(stock.ExcelData.PE),
        //        EstimateFairValue = ZScoreCalculator.EstimateFairValue(stock.ExcelData.EPS, stock.ExcelData.PE),


        //        HistRsi = rsiList,
        //        HistMacdLine = macdData.MacdLine,
        //        HistMacdHistogram = macdData.Histogram,
        //        HistStochastic = stochList,
        //        HistCci = cciList,
        //        HistMomentum = momList,
        //        HistObv = obvList,
        //        HistVwMacd = vwmacdList,
        //        HistCmf = cmfList,
        //        HistMfi = mfiList,
        //    };
        //}

        public static MarketDerivedMetrics Build(EnrichedStockData stock)
        {
            var dailyCandles = CalculationUtils.SortAscending(stock.DailyCandles);
            var latest = CalculationUtils.Latest(dailyCandles);
            var previous = CalculationUtils.Previous(dailyCandles);

            // --- مکانیزم اولویت‌بندی قیمت ---
            // اولویت ۱: قیمت فایل اکسل (اگر موجود باشد) | اولویت ۲: قیمت آخرین کندل
            var lastPrice = (stock.ExcelData != null && stock.ExcelData.LastPrice > 0)
                ? stock.ExcelData.LastPrice
                : (latest?.Close ?? 0);

            var closingPrice = (stock.ExcelData != null && stock.ExcelData.ClosingPrice > 0)
                ? stock.ExcelData.ClosingPrice
                : (latest?.Close ?? 0);

            // --- محاسبات تکنیکال پایه ---
            var sma20 = MovingAverageCalculator.SMA(dailyCandles, 20);
            var sma50 = MovingAverageCalculator.SMA(dailyCandles, 50);
            var sma200 = MovingAverageCalculator.SMA(dailyCandles, 200);

            var avgVolume20 = VolumeCalculator.AverageVolume(dailyCandles, 20);
            var avgVolume30 = VolumeCalculator.AverageVolume(dailyCandles, 30);
            var avgVolume90 = VolumeCalculator.AverageVolume(dailyCandles, 90);

            // اولویت حجم: تابلو (اکسل) یا کندل
            var volumeToday = (stock.ExcelData != null && stock.ExcelData.VolumeToday > 0)
                ? stock.ExcelData.VolumeToday
                : (latest?.Volume ?? 0);

            var atr14 = RiskCalculator.ATR(dailyCandles, 14);
            var support = SupportResistanceCalculator.FindMonthlySupport(stock.MonthlyCandles);
            var resistance = SupportResistanceCalculator.FindMonthlyResistance(stock.MonthlyCandles);

            // --- محاسبات اندیکاتورهای تاریخی (برای واگرایی) ---
            var rsiList = MomentumCalculator.CalculateRsiList(dailyCandles, 14);
            var macdData = MomentumCalculator.CalculateMacd(dailyCandles, 12, 26, 9);
            var stochList = MomentumCalculator.CalculateStochastic(dailyCandles, 14);
            var cciList = MomentumCalculator.CalculateCci(dailyCandles, 20);
            var momList = MomentumCalculator.CalculateMomentum(dailyCandles, 10);
            var obvList = MomentumCalculator.CalculateObv(dailyCandles);
            var vwmacdList = MomentumCalculator.CalculateVwMacd(dailyCandles, 12, 26);
            var cmfList = MomentumCalculator.CalculateCmf(dailyCandles, 20);
            var mfiList = MomentumCalculator.CalculateMfiList(dailyCandles, 14);

            return new MarketDerivedMetrics
            {
                LastPrice = lastPrice,
                ClosingPrice = closingPrice,
                FirstPrice = (stock.ExcelData != null) ? stock.ExcelData.FirstPrice : (latest?.Open ?? 0),
                High = (stock.ExcelData != null) ? stock.ExcelData.High : (latest?.High ?? 0),
                Low = (stock.ExcelData != null) ? stock.ExcelData.Low : (latest?.Low ?? 0),

                LastPriceChangePercent = CalculationUtils.ChangePercent(lastPrice, previous?.Close ?? 0),
                ClosingPriceChangePercent = CalculationUtils.ChangePercent(closingPrice, previous?.Close ?? 0),

                // ... بازدهی‌ها ...
                Return1Month = ReturnCalculator.Return(dailyCandles, 21),
                Return3Month = ReturnCalculator.Return(dailyCandles, 63),
                Return6Month = ReturnCalculator.Return(dailyCandles, 126),
                Return1Year = ReturnCalculator.Return(dailyCandles, 252),

                SMA20 = sma20,
                SMA50 = sma50,
                SMA200 = sma200,
                DistanceToSMA20 = CalculationUtils.DistancePercent(lastPrice, sma20),
                DistanceToSMA50 = CalculationUtils.DistancePercent(lastPrice, sma50),
                DistanceToSMA200 = CalculationUtils.DistancePercent(lastPrice, sma200),

                RSI14 = rsiList.LastOrDefault(),
                MFI14 = mfiList.LastOrDefault(),

                VolumeToday = volumeToday,
                AvgVolume20 = avgVolume20,
                AvgVolume30 = avgVolume30,
                AvgVolume90 = avgVolume90,

                VolumeRatio20 = CalculationUtils.SafeDivide(volumeToday, avgVolume20),
                VolumeRatio30 = CalculationUtils.SafeDivide(volumeToday, avgVolume30),
                VolumeRatio90 = CalculationUtils.SafeDivide(volumeToday, avgVolume90),
                VolumeExpansionScore = VolumeCalculator.VolumeExpansionScore(dailyCandles),

                // --- متریک‌های تابلویی با Fallback (اگر اکسل نال بود) ---
                DemandToTradeRatio = (stock.ExcelData != null) ? CalculateDemandToTradeRatio(stock.ExcelData) : 1.0m,
                QueuePower = (stock.ExcelData != null) ? CalculateQueuePower(stock.ExcelData) : 1.0m,
                RealMoneyFlow = (stock.ExcelData != null) ? CalculateRealMoneyFlow(stock.ExcelData) : 0,
                AccumulationScore = (stock.ExcelData != null) ? CalculateAccumulationScore(stock.ExcelData) : 50,

                // ... سایر متریک‌های تکنیکال ...
                DistanceToSupport = SupportResistanceCalculator.DistanceToSupport(lastPrice, support),
                DistanceToResistance = SupportResistanceCalculator.DistanceToResistance(lastPrice, resistance),

                ATR14 = atr14,
                ATR14Percent = lastPrice > 0 ? (atr14 / lastPrice) * 100 : 0,

                // ... قدرت نسبی و صنعت ...
                SectorRotationScore = SectorRotationCalculator.Score(stock.IndustryIndexCandles),
                ZScoreIndustry = ZScoreCalculator.StockVsIndustryZScore(dailyCandles, stock.IndustryIndexCandles),

                // ارزش‌گذاری (با هندل کردن نال بودن اکسل)
                ValueScore = (stock.ExcelData != null) ? ZScoreCalculator.CalculateValueScore(stock.ExcelData.PE) : 0,
                EstimateFairValue = (stock.ExcelData != null) ? ZScoreCalculator.EstimateFairValue(stock.ExcelData.EPS, stock.ExcelData.PE) : 0,

                // آرایه‌های واگرایی
                HistRsi = rsiList,
                HistMacdLine = macdData.MacdLine,
                HistMacdHistogram = macdData.Histogram,
                HistStochastic = stochList,
                HistCci = cciList,
                HistMomentum = momList,
                HistObv = obvList,
                HistVwMacd = vwmacdList,
                HistCmf = cmfList,
                HistMfi = mfiList,
            };
        }

        private static decimal CalculateAccumulationScore(ExcelStockRow row)
        {
            decimal score = 0;

            if (row.RealBuyPercent > 60)
                score += 25;

            if (row.RealSellPercent < 50)
                score += 25;

            if (row.DemandVolume > row.SupplyVolume)
                score += 25;

            if (row.LastPricePercent > 0)
                score += 25;

            return score;
        }

        private static decimal CalculateDemandToTradeRatio(ExcelStockRow row)
        {
            if (row.VolumeToday <= 0)
                return 0;

            return row.DemandVolume / row.VolumeToday;
        }

        private static decimal CalculateQueuePower(ExcelStockRow row)
        {
            var demand =
                row.DemandVolume *
                row.DemandPrice;

            var supply =
                row.SupplyVolume *
                row.SupplyPrice;

            if (supply <= 0)
                return 999;

            return demand / supply;
        }

        private static decimal CalculateRealMoneyFlow(ExcelStockRow row)
        {
            if (row.VolumeToday <= 0)
                return 0;

            return
                (row.RealBuy - row.RealSell)
                /
                row.VolumeToday;
        }
    }
}