namespace MarketIntelligence.Application
{
    public sealed class ExcelStockRow
    {
        /// <summary>
        /// نماد
        /// </summary>
        public string Symbol { get; set; }
        /// <summary>
        /// حجم معاملات
        /// </summary>
        public decimal VolumeToday { get; set; }        
        /// <summary>
        /// اولین قیمت
        /// </summary>
        public decimal FirstPrice { get; set; }
        /// <summary>
        /// آخرین قیمت
        /// </summary>
        public decimal LastPrice { get; set; }
        /// <summary>
        /// درصد آخرین قیمت
        /// </summary>
        public decimal LastPricePercent { get; set; }
        /// <summary>
        /// قیمت پایانی
        /// </summary>

        public decimal ClosingPrice { get; set; }
        /// <summary>
        /// درصد قیمت پایانی
        /// </summary>
        public decimal ClosingPricePercent { get; set; }
        
        /// <summary>
        /// بالاترین
        /// </summary>
        public decimal High { get; set; }
        /// <summary>
        /// پایین‌ترین
        /// </summary>
        public decimal Low { get; set; }
        
        
        /// <summary>
        /// بازدهی1ماه
        /// </summary>
        public decimal Return1Month { get; set; }
        /// <summary>
        /// بازدهی3ماه
        /// </summary>
        public decimal Return3Month { get; set; }
        /// <summary>
        /// بازدهی6ماه
        /// </summary>
        public decimal Return6Month { get; set; }
        /// <summary>
        /// بازدهی1سال
        /// </summary>
        public decimal Return1Year { get; set; }
        /// <summary>
        /// حجم30روزه
        /// </summary>
        public decimal AvgVolume30 { get; set; }
        /// <summary>
        /// حجم90روزه
        /// </summary>
        public decimal AvgVolume90 { get; set; }
        /// <summary>
        /// ضریب بتا
        /// </summary>
        public decimal Beta { get; set; }
        /// <summary>
        /// RSI
        /// </summary>
        public decimal RSI { get; set; }
        /// <summary>
        /// MFI
        /// </summary>
        public decimal MFI { get; set; }
        /// <summary>
        /// SMA20d
        /// </summary>
        public decimal SMA20 { get; set; }
        /// <summary>
        /// SMA50d
        /// </summary>
        public decimal SMA50 { get; set; }
        /// <summary>
        /// SMA200d
        /// </summary>
        public decimal SMA200 { get; set; }


        ////***********    اگه بشه این دیتا ها رو هم بگیریم عالیه         ************///
        
        /// <summary>
        /// تعداد معاملات
        /// </summary>
        public decimal TradeCount { get; set; }
        /// <summary>
        /// ارزش معاملات
        /// </summary>
        public decimal TradeValue { get; set; }

        /// <summary>
        /// حجم تقاضا
        /// </summary>
        public decimal DemandVolume { get; set; }
        /// <summary>
        /// قیمت تقاضا
        /// </summary>
        public decimal DemandPrice { get; set; }
        /// <summary>
        /// قیمت عرضه
        /// </summary>
        public decimal SupplyPrice { get; set; }
        /// <summary>
        /// حجم عرضه
        /// </summary>
        public decimal SupplyVolume { get; set; }
        /// <summary>
        /// آستانه بال
        /// </summary>
        public decimal UpperThreshold { get; set; }
        /// <summary>
        /// آستانه پایین
        /// </summary>
        public decimal LowerThreshold { get; set; }
        /// <summary>
        /// خرید حقیقی
        /// </summary>
        public decimal RealBuy { get; set; }
        /// <summary>
        /// درصد خرید حقیقی
        /// </summary>
        public decimal RealBuyPercent { get; set; }
        /// <summary>
        /// خرید حقوقی
        /// </summary>
        public decimal LegalBuy { get; set; }
        /// <summary>
        /// درصد خرید حقوقی
        /// </summary>
        public decimal LegalBuyPercent { get; set; }
        /// <summary>
        /// فروش حقیقی
        /// </summary>
        public decimal RealSell { get; set; }
        /// <summary>
        /// درصد فروش حقیقی
        /// </summary>
        public decimal RealSellPercent { get; set; }
        /// <summary>
        /// فروش حقوقی
        /// </summary>
        public decimal LegalSell { get; set; }
        /// <summary>
        /// درصد فروش حقوقی
        /// </summary>
        public decimal LegalSellPercent { get; set; }

        /////***********        دیتاهای بنیادی که از کندل در نمیاد
        
        /// <summary>
        /// حباب
        /// </summary>
        public decimal Bubble { get; set; }
        /// <summary>
        /// EPS
        /// </summary>
        public decimal EPS { get; set; }
        /// <summary>
        /// P/E
        /// </summary>
        public decimal PE { get; set; }
        /// <summary>
        /// DPS درصد
        /// </summary>
        public decimal DPSPercent { get; set; }
        /// <summary>
        /// P/B
        /// </summary>
        public decimal PB { get; set; }
        /// <summary>
        /// P/S
        /// </summary>
        public decimal PS { get; set; }
        /// <summary>
        /// P/NAV
        /// </summary>
        public decimal PNAV { get; set; }
    }
}
