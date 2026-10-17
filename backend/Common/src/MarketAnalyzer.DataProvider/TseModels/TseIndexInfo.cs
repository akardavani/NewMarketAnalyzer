using System;

namespace MarketAnalyzer.DataProvider.TseModels
{
    public class TseIndexInfo
    {
        /// <summary>
        /// کد شاخص (InsCode)
        /// </summary>
        public long InsCode { get; set; }

        /// <summary>
        /// تاریخ و زمان (DEven + HEven)
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// مقدار شاخص (XNivInuClMresIbs)
        /// </summary>
        public decimal LastValue { get; set; }

        /// <summary>
        /// مقدار اولين انتشار شاخص (XNivInuPrDifMresIbs)
        /// </summary>
        public decimal FirstValue { get; set; }

        /// <summary>
        /// زمان اولين انتشار شاخص (HNivInuPrDifMresIbs)
        /// </summary>
        public TimeSpan FirstValueTime { get; set; }

        /// <summary>
        /// بيشترين شاخص در طول روز (XNivInuPhMresIbs)
        /// </summary>
        public decimal HighestValue { get; set; }

        /// <summary>
        /// درصد تغييرات بيشترين مقدار شاخص بر حسب روز قبل (XVarIdxPhJClV)
        /// </summary>
        public decimal HighestValueChangePercent { get; set; }

        /// <summary>
        /// زمان بيشترين شاخص در طول روز (HNivInuPhMresIbs)
        /// </summary>
        public TimeSpan HighestValueTime { get; set; }

        /// <summary>
        /// کمترين شاخص در طول روز (XNivInuPbMresIbs)
        /// </summary>
        public decimal LowestValue { get; set; }

        /// <summary>
        /// درصد تغييرات کمترين مقدار شاخص بر حسب روز قبل (XVarIdxPbJClV)
        /// </summary>
        public decimal LowestValueChangePercent { get; set; }

        /// <summary>
        /// زمان کمترين شاخص در طول روز (HNivInuPbMresIbs)
        /// </summary>
        public TimeSpan LowestValueTime { get; set; }

        /// <summary>
        /// درصد تغيير نمادهاي کاهش يافته در شاخص
        /// </summary>
        public decimal InstrumentsDecreasedChangePercent { get; set; }

        /// <summary>
        /// مقدار سود خالص پرداختي نمادهاي شاخص در روز جاري
        /// </summary>
        public decimal InstrumentsDividend { get; set; }

        /// <summary>
        /// مقدار سرمايه ديروز شاخص
        /// </summary>
        public decimal LastDayCapital { get; set; }

        /// <summary>
        /// ضريب تنظيم براي سرمايه پايه شاخص
        /// </summary>
        public decimal AdjustmentCoefficient { get; set; }

        /// <summary>
        /// مقدار شاخص بازده خالص نقدي
        /// </summary>
        public decimal NetCashValue { get; set; }
    }
}
