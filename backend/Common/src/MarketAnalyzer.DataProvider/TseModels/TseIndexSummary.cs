using System;

namespace MarketAnalyzer.DataProvider.TseModels
{
    public class TseIndexSummary
    {
        /// <summary>
        /// کد داخلي نماد (InsCode)
        /// </summary>
        public long InsCode { get; set; }
        /// <summary>
        /// نام 30 رقمي فارسي نماد (LVal30)
        /// </summary>
        public string InstrumentName { get; set; }
        /// <summary>
        /// تاریخ و ساعت رویداد (DEven + HEven)
        /// </summary>
        public DateTime EventDate { get; set; }
        /// <summary>
        /// تعداد نمادهاي معامله شده در شاخص (ZValIdxCot)
        /// </summary>
        public int TradedInstrumentsNumber { get; set; }
        /// <summary>
        /// تعداد نمادهاي کاهش يافته در شاخص (ZValBaiIbs)
        /// </summary>
        public int DecliningInstrumentsNumber { get; set; }
        /// <summary>
        /// تعداد نمادهاي افزايش يافته در شاخص (ZValHauIbs)
        /// </summary>
        public int RisingInstrumentsNumber { get; set; }
        /// <summary>
        /// تعداد نمادهاي بدون تغيير در شاخص (ZValIchgIbs)
        /// </summary>
        public int UnchangedInstrumentsNumber { get; set; }
        /// <summary>
        /// تعداد نمادهاي معامله نشده در شاخص (ZValNonCotIbs)
        /// </summary>
        public int NonTradedInstrumentsNumber { get; set; }
        /// <summary>
        /// تعداد نمادهاي رزرو شاخص (ZValResIbs)
        /// </summary>
        public int ReservedInstrumentsNumber { get; set; }
        /// <summary>
        /// تعداد نمادهاي معلق در شاخص (ZValSuIbs)
        /// </summary>
        public int SuspendedInstrumentsNumber { get; set; }
        /// <summary>
        /// تعداد کل نمادها در شاخص (ZTotValIbs)
        /// </summary>
        public int TotalInstrumentsNumber { get; set; }
        /// <summary>
        /// زمان بیشترین مقدار شاخص (HPhJIdx)
        /// </summary>
        public TimeSpan HighestValueTime { get; set; }
        /// <summary>
        /// زمان کمترین مقدار شاخص (HPbJIdx)
        /// </summary>
        public TimeSpan LowestValueTime { get; set; }
        /// <summary>
        /// آخرين مقدار شاخص روز (XDrNivJIdx004)
        /// </summary>
        public decimal LastValue { get; set; }
        /// <summary>
        /// بيشترين مقدار شاخص روز (XPhNivJIdx004)
        /// </summary>
        public decimal HighestValue { get; set; }
        /// <summary>
        /// کمترين مقدار شاخص روز (XPbNivJIdx004)
        /// </summary>
        public decimal LowestValue { get; set; }
        /// <summary>
        /// مقدار شاخص بازده نقدي (XNivIrteNetIbs)
        /// </summary>
        public decimal NetReturn { get; set; }
        /// <summary>
        /// درصد تغييرات شاخص در طول روز (XVarIdxJ)
        /// </summary>
        public decimal DayVariation { get; set; }
        /// <summary>
        /// درصد تغييرات شاخص در طول روز نسبت به روز قبل (XVarIdxJRfV)
        /// </summary>
        public decimal PreviousDayVariation { get; set; }
        /// <summary>
        /// درصد ميانگين تغيير نمادها در شاخص (XMoyVarValIbs)
        /// </summary>
        public decimal InstrumentsVariationAverage { get; set; }
        /// <summary>
        /// درصد ميانگين تغيير نمادهاي کاهش يافته در شاخص (XMoyVarValBaiIbs)
        /// </summary>
        public decimal DecliningInstrumentsVariationAverage { get; set; }
        /// <summary>
        /// درصد ميانگين تغيير نمادهاي افزايش يافته در شاخص (XmoyVarValHauIbs)
        /// </summary>
        public decimal RisingInstrumentsVariationAverage { get; set; }
        /// <summary>
        /// مقدار تغييرات شاخص در طول روز نسبت به روز قبل
        /// </summary>
        public decimal ChangeValue => LastValue - Math.Round(LastValue / (100 + PreviousDayVariation) * 100, 2);
    }
}
