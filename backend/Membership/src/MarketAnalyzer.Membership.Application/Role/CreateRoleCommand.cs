using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Application
{
    public class CreateRoleCommand : IRequest<int>
    {
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
