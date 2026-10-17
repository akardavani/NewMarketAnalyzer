namespace MarketAnalyzer.Sejam.Models
{
    public class SejamBrokerEntry
    {
        /// <summary>
        /// شناسه
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// عنوان کارگزاری
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// کد تعیین شده برای کارگزاری
        /// </summary>
        public string Code { get; set; }
    }
}