namespace MarketAnalyzer.SharedKernel
{
    /// <summary>
    /// مشخصات حصاب تفصیلی ایجاد شده توسط دامنه‌ی حسابداری
    /// </summary>
    public class DetailAccountInfo
    {
        /// <summary>
        /// شناسه حساب
        /// </summary>
        public required int Id { get; init; }
        /// <summary>
        /// کد حساب
        /// </summary>
        public required string Code { get; init; }
        /// <summary>
        /// عنوان حساب
        /// </summary>
        public required string Title { get; init; }
        /// <summary>
        /// شناسه منحصر به فرد اصلی
        /// </summary>
        public required string PrimaryIdentifier { get; init; }
        /// <summary>
        /// شناسه منحصر به فرد ثانویه
        /// </summary>
        public required string SecondaryIdentifier { get; init; }
    }
}