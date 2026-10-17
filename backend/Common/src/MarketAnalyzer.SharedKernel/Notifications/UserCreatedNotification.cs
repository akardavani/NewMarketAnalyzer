using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.SharedKernel
{
    public class UserCreatedNotification : INotification
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public required int UserId { get; init; }
        /// <summary>
        /// نام کاربری
        /// </summary>
        public required string UserName { get; init; }
        /// <summary>
        /// رمز عبور
        /// </summary>
        public required string Password { get; init; }
        /// <summary>
        /// نام
        /// </summary>
        public required string FullName { get; init; }
        /// <summary>
        /// آدرس ایمیل
        /// </summary>
        public required string Email { get; init; }
        /// <summary>
        /// آدرس ایمیل اعتبارسنجی شده یا خیر
        /// </summary>
        public required bool EmailConfirmed { get; init; }
        /// <summary>
        /// شماره موبایل
        /// </summary>
        public required string PhoneNumber { get; init; }
    }
}