using System.ComponentModel;

namespace MarketIntelligence.Domain
{
    public enum SourceType
    {
        // Tse
        [LookupInfo("Nahayatnegar", "نهایت نگر")]
        Nahayatnegar = 10,

        [Description("بورس تهران")]
        [LookupInfo("Tse", "بورس تهران")]
        Tse = 11,


        // Crypto
        [LookupInfo("Kucoin", "کوکوین")]
        Kucoin = 20,

        [LookupInfo("Binance", "بایننس")]
        Binance = 21,

        // Macro
        [LookupInfo("tgju", "tgju")]
        tgju = 30,


        // CryptoToman
        [LookupInfo("Nobitex", "نوبیتکس")]
        Nobitex = 40,

        [LookupInfo("Wallex", "والکس")]
        Wallex = 41,


        // RealEstate
        [LookupInfo("Kilid", "کلید")]
        Kilid = 50,

        //// Car
        //[LookupInfo("Hamrah", "همراه")]
        //Hamrah = 60,


        // InvestmentFund
        [LookupInfo("Fipiran", "فیپ ایران")]
        Fipiran = 70,


        [LookupInfo("Ime", "کالا")]
        Ime = 80,
    }
}
