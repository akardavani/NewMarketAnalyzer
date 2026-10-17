namespace MarketAnalyzer.Domain
{
    public class TseTmcETFNav : IEntity
    {
        public string Id { get; set; }
        public long InsCode { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public long Date { get; set; }
        /// <summary>
        /// زمان
        /// </summary>
        public long Time { get; set; }
        /// <summary>
        /// تاریخ زمان
        /// </summary>
        public DateTime DateTime { get; set; }
        /// <summary>
        /// Nav ابطال
        /// </summary>
        public decimal CancellationNav { get; set; }
        /// <summary>
        /// Nav صدور
        /// </summary>
        public decimal IssuanceNav { get; set; }
        /// <summary>
        /// بسته؟
        /// </summary>
        public int IsClose { get; set; }
    }
}
