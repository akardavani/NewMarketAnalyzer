namespace MarketIntelligence.Application
{
    public class IranQueueResult
    {
        /// <summary>
        /// نماد
        /// </summary>
        public string Symbol { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InstrumentId { get; set; }
        public string InsCode { get; set; }
        /// <summary>
        /// نسبت 
        /// </summary>
        public decimal Ratio { get; set; }
        /// <summary>
        /// حجم معاملات
        /// </summary>
        public decimal VolumeToday { get; set; }
        /// <summary>
        /// حجم تقاضا
        /// </summary>
        public decimal DemandVolume { get; set; }
        /// <summary>
        /// بازدهی1ماه
        /// </summary>
        public decimal Return1Month { get; set; }
        /// <summary>
        /// حجم30روزه
        /// </summary>
        public decimal AvgVolume30 { get; set; }       
        
    }
}
