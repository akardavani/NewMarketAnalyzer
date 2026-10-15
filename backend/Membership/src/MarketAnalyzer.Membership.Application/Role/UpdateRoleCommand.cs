using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Application
{
    public class UpdateRoleCommand : IRequest<int>
    {
        /// <summary>
        /// شناسه
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
    }
}
