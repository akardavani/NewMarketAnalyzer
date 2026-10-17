using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Domain.Notifications
{
    /// <summary>
    /// رویداد ارسال کلمه عبور به کاربر
    /// <para>برای ارسال کلمه عبور به همراه نام کاربری از طریق ایمیل و پیامک، این رویداد منشتر میشود</para>
    /// </summary>
    public class UserPasswordResetedNotification : INotification
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
