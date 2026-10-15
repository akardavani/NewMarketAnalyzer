using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Cms.Application
{
    public class CreateContactUsCommand : IRequest<int>
    {
        /// <summary>
        /// نام و نام خانوادگی
        /// </summary>
        public required string FullName { get; init; }
        /// <summary>
        /// ایمیل
        /// </summary>
        public required string Email { get; init; }
        /// <summary>
        /// شماره تماس
        /// </summary>
        public required string PhoneNumber { get; init; }
        /// <summary>
        /// موضوع
        /// </summary>
        public required string Subject { get; init; }
        /// <summary>
        /// متن پیام
        /// </summary>
        public required string Text { get; init; }
        /// <summary>
        /// IP
        /// </summary>
        public required string IpAddress { get; init; }
    }
}
