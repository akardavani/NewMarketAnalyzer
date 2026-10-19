namespace MarketAnalyzer.MarketIntelligence.Domain
{
    public class FundNavGap : IEntity
    {
        public string Id { get; set; }              // کلید اصلی
        public string RegNo { get; set; }        // شماره ثبت صندوق
        public string FundInstrumentId { get; set; }
        public DateTime Date { get; set; }       // تاریخ وقوع گپ
        public decimal PreviousNav { get; set; } // مقدار قبلی
        public decimal CurrentNav { get; set; }  // مقدار فعلی
        public decimal Ratio { get; set; }       // نسبت تغییر (برای تشخیص شدت گپ)
    }
}
