using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.SharedKernel.Notifications
{
    public class CreateDetailAccountRequestedNotification : INotification
    {
        /// <summary>
        /// کد حساب تفصیلی
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
        /// <summary>
        /// شاخص تعیین نوع حساب تفصیلی
        /// </summary>
        public required DetailAccountTypeIndicator Indicator { get; init; }
        /// <summary>
        /// شرح
        /// </summary>
        public required string Description { get; init; }
    }
}
