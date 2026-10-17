namespace MarketIntelligence.Domain
{
    public enum MarketType
    {
        /// <summary>
        /// بازار تهران
        /// </summary>
        [LookupInfo("Tse", "بورس تهران")]
        Tse = 1,

        /// <summary>
        /// طلا و دلار
        /// </summary>
        [LookupInfo("Macro", "اقتصاد کلان")]
        Macro = 2,

        /// <summary>
        /// کریپتو
        /// </summary>
        [LookupInfo("Crypto", "رمزارز")]
        Crypto = 3,

        /// <summary>
        /// کالا
        /// </summary>
        [LookupInfo("Ime", "بورس کالا")]
        Ime = 4,

        /// <summary>
        /// صندوق سرمایه گذاری
        /// </summary>
        [LookupInfo("InvestmentFund", "صندوق سرمایه‌گذاری")]
        InvestmentFund = 5,

        /// <summary>
        /// خونه
        /// </summary>
        [LookupInfo("RealEstate", "مسکن")]
        RealEstate = 6,

        /// <summary>
        /// شاخص بازار تهران
        /// </summary>
        [LookupInfo("TseIndex", "شاخص بورس")]
        TseIndex = 7,

        /// <summary>
        /// کریپتو تومان
        /// </summary>
        [LookupInfo("CryptoToman", "کریپتو تومان")]
        CryptoToman = 8
    }
}
