namespace MarketAnalyzer.SharedKernel
{
    public class PillarInfo
    {
        /// <summary>
        /// شناسه شخص
        /// </summary>
        public required long PersonId { get; init; }
        /// <summary>
        /// نام کامل شخص
        /// </summary>
        public required string FullName { get; init; }
        /// <summary>
        /// کد ملی/شناسه ملی شرکت/پم کد سبد/شناسه اتباع خارجه
        /// </summary>
        public required string NationalCode { get; init; }
        /// <summary>
        /// کد حساب تفصیلی
        /// </summary>
        public required string DetailAccountCode { get; init; }
        /// <summary>
        /// شناسه نوع شخص
        /// </summary>
        public required int InvestorTypeId { get; init; }
        /// <summary>
        /// نوع شخص
        /// </summary>
        public required string InvestorTypeName { get; init; }
        /// <summary>
        /// شناسه نقش
        /// </summary>
        public required long RoleId { get; init; }
        /// <summary>
        /// نام نقش
        /// </summary>
        public required string RoleName { get; init; }
    }
}
