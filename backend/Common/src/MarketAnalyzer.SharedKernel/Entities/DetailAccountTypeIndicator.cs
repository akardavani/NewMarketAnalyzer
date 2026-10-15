using MarketAnalyzer.Abstractions;
using System.Linq;

namespace MarketAnalyzer.SharedKernel
{
    public class DetailAccountTypeIndicator : Enumeration
    {
        /// <summary>
        /// بانک‌ها
        /// </summary>
        public static readonly DetailAccountTypeIndicator Banks = new(1, "بانک‌ها");
        /// <summary>
        /// اشخاص
        /// </summary>
        public static readonly DetailAccountTypeIndicator Persons = new(2, "اشخاص");
        /// <summary>
        /// سهام
        /// </summary>
        public static readonly DetailAccountTypeIndicator Stocks = new(3, "سهام");
        /// <summary>
        /// شرکت‌ها
        /// </summary>
        public static readonly DetailAccountTypeIndicator Companies = new(4, "شرکت‌ها");
        /// <summary>
        /// کارگزاری‌ها
        /// </summary>
        public static readonly DetailAccountTypeIndicator Brokers = new(5, "کارگزاری‌ها");
        /// <summary>
        /// نوع هزینه
        /// </summary>
        public static readonly DetailAccountTypeIndicator CostType = new(6, "نوع هزینه");
        /// <summary>
        /// نوع نماد
        /// </summary>
        public static readonly DetailAccountTypeIndicator InstrumentType = new(7, "نوع نماد");
        /// <summary>
        /// نقش رکن
        /// </summary>
        public static readonly DetailAccountTypeIndicator PillarRole = new(8, "نقش رکن");
        /// <summary>
        /// هدف از انتشار اوراق تبعی
        /// </summary>
        public static readonly DetailAccountTypeIndicator EmbeddedsIssuingPurpose = new(9, "هدف از انتشار اوراق تبعی");
        /// <summary>
        /// سایر
        /// </summary>
        public static readonly DetailAccountTypeIndicator Others = new(99, "سایر");

        private DetailAccountTypeIndicator(int id, string name) : base(id, name)
        {

        }

        public static explicit operator DetailAccountTypeIndicator(int id) =>
            GetValues<DetailAccountTypeIndicator>().First(e => e.Id == id);
    }
}
