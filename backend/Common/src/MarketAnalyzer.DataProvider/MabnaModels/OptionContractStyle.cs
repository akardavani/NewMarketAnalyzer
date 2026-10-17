namespace MarketAnalyzer.DataProvider.MabnaModels
{
    /// <summary>
    /// سبک اختیار معامله
    /// </summary>
    public class OptionContractStyle : EntityBase
    {
        /// <summary>
        /// عنوان
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// عنوان لاتین
        /// </summary>
        public string EnglishTitle { get; set; }
    }
}
