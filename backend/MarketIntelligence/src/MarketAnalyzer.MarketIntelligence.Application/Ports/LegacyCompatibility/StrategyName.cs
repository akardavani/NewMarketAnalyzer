namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public static class StrategyName
    {
        public static string IranBaseRelation => "IranBaseRelation";
        /// <summary>
        /// پیدا کردن بهترین سهم ها در بازار تهران
        /// با گرفتن روزانه اکسل از ایزی و تجمیع اش با دیتاهای دیگه
        /// </summary>
        public static string IranEquityMain => "IranEquityMain";
        /// <summary>
        /// واگرایی اندیکاتورها
        /// </summary>
        public static string MultiIndicatorDivergence => "MultiIndicatorDivergence";
        public static string IranQueue => "IranQueue";
        /// <summary>
        /// آتی طلا و نقره
        /// </summary>
        public static string YtmFuture => "YtmFuture";
        /// <summary>
        /// نسبت نمادها
        /// </summary>
        public static string RatioCandles => "RatioCandles";
        /// <summary>
        /// برگشت از کف
        /// </summary>
        public static string BottomReversal => "BottomReversal";
    }    
}
