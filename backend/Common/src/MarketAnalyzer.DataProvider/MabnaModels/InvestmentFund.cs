using System;

namespace MarketAnalyzer.DataProvider.MabnaModels
{
    /// <summary>
    /// صندوق سرمایه گذاری
    /// </summary>
    public class InvestmentFund : EntityBase
    {
        /// <summary>
        /// کد
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// نام صندوق
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// نام لاتین
        /// </summary>
        public string EnglishName { get; set; }
        /// <summary>
        /// نام کوتاه
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// نام کوتاه انگلیسی
        /// </summary>
        public string EnglishShortName { get; set; }
        /// <summary>
        /// نماد معاملاتی
        /// </summary>
        public string TradeSymbol { get; set; }
        /// <summary>
        /// تاریخ شروع به کار
        /// </summary>
        public DateTime InceptionDate { get; set; }
        /// <summary>
        /// مدیر
        /// </summary>
        public PersonEntity Manager { get; set; }
        /// <summary>
        /// مدیر سرمایه گذاری
        /// </summary>
        public PersonEntity InvestmentManager { get; set; }
        /// <summary>
        /// متولی
        /// </summary>
        public PersonEntity Custodian { get; set; }
        /// <summary>
        /// ضامن نقد شوندگی
        /// </summary>
        public PersonEntity LiquidityGuarantor { get; set; }
        /// <summary>
        /// وضعیت صندوق
        /// </summary>
        public FundState State { get; set; }
    }
}
