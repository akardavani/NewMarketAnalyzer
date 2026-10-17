namespace MarketIntelligence.Domain
{
    public enum Timeframe
    {
        /// <summary>
        /// ماهانه
        /// </summary>
        [LookupInfo("Monthly", "ماهانه")]
        Mountly = 1,
        /// <summary>
        /// هفتگی
        /// </summary>
        [LookupInfo("Weekly", "هفتگی")]
        Weekly = 2,
        /// <summary>
        /// روزانه
        /// </summary>
        [LookupInfo("Daily", "روزانه")]
        Daily = 3,
        /// <summary>
        /// چهار ساعته
        /// </summary>
        [LookupInfo("4H", "۴ ساعته")]
        FourHours = 4,
        /// <summary>
        /// یکساعته
        /// </summary>
        [LookupInfo("1H", "۱ ساعته")]
        OneHour = 5,
        /// <summary>
        /// پانزده دقیقه
        /// </summary>
        [LookupInfo("15M", "۱۵ دقیقه")]
        FifteenMinutes = 6,
        /// <summary>
        /// پنج دقیقه
        /// </summary>
        [LookupInfo("5M", "۵ دقیقه")]
        FiveMinutes = 7
    }
}
