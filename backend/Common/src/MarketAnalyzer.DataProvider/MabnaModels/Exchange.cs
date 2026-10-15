namespace MarketAnalyzer.DataProvider.MabnaModels
{
    public class Exchange : EntityBase
    {
        /// <summary>
        /// کد بازار
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// نام فارسی بازار
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// نام انگلیسی بازار
        /// </summary>
        public string EnglishTitle { get; set; }
    }
}
