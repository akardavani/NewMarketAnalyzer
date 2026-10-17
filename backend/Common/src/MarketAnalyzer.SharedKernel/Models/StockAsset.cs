namespace MarketAnalyzer.SharedKernel
{
    /// <summary>
    /// اطلاعات نمادهای بورسی
    /// <para>برای استفاده در گزارش ترکیب دارایی‌ها</para>
    /// </summary>
    public class StockAsset
    {
        /// <summary>
        /// شناسه نماد
        /// </summary>
        public required string InstrumentId { get; init; }
        /// <summary>
        /// شناسه بازار
        /// </summary>
        public required int ExchangeId { get; init; }
        /// <summary>
        /// نام بازار
        /// </summary>
        public required string Exchange { get; init; }
        /// <summary>
        /// کد صنعت
        /// </summary>
        public required string SectorCode { get; init; }
        /// <summary>
        /// نام صنعت
        /// </summary>
        public required string SectorName { get; init; }
        /// <summary>
        /// کد تابلو
        /// </summary>
        public required string BoardCode { get; init; }
        /// <summary>
        /// نام تابلو
        /// </summary>
        public required string BoardName { get; init; }
        /// <summary>
        /// نام کامل نماد
        /// </summary>
        public required string FullName { get; init; }
        /// <summary>
        /// نام نماد
        /// </summary>
        public required string NativeName { get; init; }
        /// <summary>
        /// دسته بندی
        /// </summary>
        public required StockAssetCategory Category { get; init; }
        /// <summary>
        /// شناسه نوع نماد
        /// </summary>
        public required int InstrumentTypeId { get; init; }
        /// <summary>
        /// نوع نماد
        /// </summary>
        public required string InstrumentType { get; init; }
        /// <summary>
        /// قیمت روز
        /// </summary>
        public required decimal DayPrice { get; init; }
        /// <summary>
        /// خالص ارزش فروش
        /// </summary>
        public required decimal NetSalesValue { get; init; }
        /// <summary>
        /// تعداد
        /// </summary>
        public required long Quantity { get; init; }
        /// <summary>
        /// سود دریافتی اوراق
        /// </summary>
        public required decimal BondsReceivedInterest { get; init; }
        /// <summary>
        /// نوع سرمایه گذاری - فقط صندوق‌های سرمایه گذاری
        /// </summary>
        public required string InvetmentType { get; init; }
        /// <summary>
        /// تبعی/سایر
        /// </summary>
        public required bool IsEmbbeded { get; init; }
        /// <summary>
        /// نوع صندوق سرمایه گذاری
        /// </summary>
        public required string FundType { get; init; }
        /// <summary>
        /// درصد از کل دارایی‌ها
        /// </summary>
        public required decimal ShareOfTotals { get; init; }
        
    }

    public enum StockAssetCategory
    {
        /// <summary>
        /// سهام / حق تقدم
        /// </summary>
        Share,
        /// <summary>
        /// اوراق
        /// </summary>
        Bond,
        /// <summary>
        /// صندوق
        /// </summary>
        Fund
    }
}
