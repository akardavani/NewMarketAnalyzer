namespace MarketAnalyzer.Membership.Application
{
    public class RolesQueryResult
    {
        /// <summary>
        /// نام
        /// </summary>
        public required int Id { get; init; }
        /// <summary>
        /// نام
        /// </summary>
        public required string Name { get; init; }
        /// <summary>
        /// نام نمایشی
        /// </summary>
        public required string DisplayName { get; init; }
        /// <summary>
        /// فعال
        /// </summary>
        public required bool IsActive { get; init; }
        /// <summary>
        /// قابل ویرایش
        /// </summary>
        public required bool Editable { get; init; }
        /// <summary>
        /// تعداد کاربران
        /// </summary>
        public required int UsersCount { get; init; }
    }
}
