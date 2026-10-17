namespace MarketAnalyzer.Membership.Application
{
    public class UsersQueryResult
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public required int Id { get; init; }
        /// <summary>
        /// نام
        /// </summary>
        public required string FullName { get; init; }
        /// <summary>
        /// نام کاربری
        /// </summary>
        public required string UserName { get; init; }
        /// <summary>
        /// شماره موبایل
        /// </summary>
        public required string PhoneNumber { get; init; }
        /// <summary>
        /// ایمیل
        /// </summary>
        public required string Email { get; init; }
        /// <summary>
        /// ورود دوعاملی
        /// </summary>
        public required bool TwoFactorAuthentication { get; init; }
        /// <summary>
        /// فعال
        /// </summary>
        public required bool IsActive { get; init; }
        /// <summary>
        /// قابل ویرایش
        /// </summary>
        public required bool Editable { get; init; }
    }
}
