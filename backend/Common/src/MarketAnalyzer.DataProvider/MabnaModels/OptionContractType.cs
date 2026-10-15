namespace MarketAnalyzer.DataProvider.MabnaModels
{
    /// <summary>
    /// نوع اختیار معامله
    /// </summary>
    public class OptionContractType : EntityBase
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
