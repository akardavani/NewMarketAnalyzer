using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.Cms.Domain.Notifications
{
    public class ContactUsAansweredNotification : INotification
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
        /// متن پیام اصلی
        /// </summary>
        public required string Text { get; init; }
        /// <summary>
        /// متن پاسخ
        /// </summary>
        public required string Reply { get; init; }
        /// <summary>
        /// پاسخ دهنده
        /// </summary>
        public required string Responder { get; init; }
        /// <summary>
        /// تاریخ پاسخگویی
        /// </summary>
        public required DateTime ReplyDateTime { get; init; }
    }
}
