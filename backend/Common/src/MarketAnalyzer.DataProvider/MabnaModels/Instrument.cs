namespace MarketAnalyzer.DataProvider.MabnaModels
{
    public class Instrument : EntityBase
    {
        /// <summary>
        /// کد بورسی
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// کد BBS
        /// </summary>
        public string BbsCode { get; set; }
        /// <summary>
        /// کد ISIN
        /// </summary>
        public string Isin { get; set; }
        /// <summary>
        /// نام نماد
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// نام انگلیسی نماد
        /// </summary>
        public string EnglishName { get; set; }
        /// <summary>
        /// نام کوتاه نماد
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// نام کوتاه انگلیسی نماد
        /// </summary>
        public string EnglishShortName { get; set; }
        /// <summary>
        /// شرکت
        /// </summary>
        public Stock Stock { get; set; }
        /// <summary>
        /// نماد اصلی (بعد از تغییر نماد، نماد جدیدی است که با همین مشخصات ثبت شده‌است)
        /// </summary>
        public Instrument MainInstrument { get; set; }

        /// <summary>
        /// شناسه شرکت
        /// </summary>
        public string CompanyId => Stock?.Company?.Id;
    }
}
