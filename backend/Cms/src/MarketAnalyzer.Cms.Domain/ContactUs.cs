using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain.Notifications;
using System;

namespace MarketAnalyzer.Cms.Domain
{
    /// <summary>
    /// تماس با ما پورتال
    /// </summary>
    public class ContactUs : AggregateRoot<int>
    {
        public ContactUs(int id) : base(id)
        {
        }

        #region [Properties]

        /// <summary>
        /// نام و نام خانوادگی
        /// </summary>
        public string FullName { get; private set; }
        /// <summary>
        /// ایمیل
        /// </summary>
        public string Email { get; private set; }
        /// <summary>
        /// شماره تماس
        /// </summary>
        public string PhoneNumber { get; private set; }
        /// <summary>
        /// موضوع
        /// </summary>
        public string Subject { get; private set; }
        /// <summary>
        /// متن پیام
        /// </summary>
        public string Text { get; private set; }
        /// <summary>
        /// IP
        /// </summary>
        public string IpAddress { get; private set; }
        /// <summary>
        /// پاسخ
        /// </summary>
        public string Reply { get; private set; }
        /// <summary>
        /// کاربر پاسخگو
        /// </summary>
        public User Responder { get; private set; }
        /// <summary>
        /// زمان پاسخگویی
        /// </summary>
        public DateTime? ReplyDateTime { get; private set; }
        /// <summary>
        /// هرزنامه
        /// </summary>
        public bool IsSpam { get; private set; }
        /// <summary>
        /// توضیحات اپراتور
        /// </summary>
        public string Description { get; private set; }

        #endregion


        public static ContactUs Create(string fullName, string email, string phoneNumber, string subject, string text, string ipAddress)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException($"'{nameof(fullName)}' cannot be null or empty.", nameof(fullName));
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException($"'{nameof(email)}' cannot be null or empty.", nameof(email));
            }

            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException($"'{nameof(subject)}' cannot be null or empty.", nameof(subject));
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException($"'{nameof(text)}' cannot be null or empty.", nameof(text));
            }

            if (string.IsNullOrEmpty(ipAddress))
            {
                throw new ArgumentException($"'{nameof(ipAddress)}' cannot be null or empty.", nameof(ipAddress));
            }

            return new ContactUs(0)
            {
                FullName = fullName,
                Email = email,
                Subject = subject,
                PhoneNumber = phoneNumber,
                Text = text,
                IpAddress = ipAddress
            };
        }

        public void MarkAsSpam()
        {
            IsSpam = true;
        }

        public void Respond(string reply, User responder, DateTime replyDateTime, string description)
        {
            if (string.IsNullOrEmpty(reply))
            {
                throw new ArgumentException($"'{nameof(reply)}' cannot be null or empty.", nameof(reply));
            }

            Reply = reply;
            Responder = responder ?? throw new ArgumentNullException(nameof(responder));
            ReplyDateTime = replyDateTime;
            Description = description;

            AddDomainEvent(() => new ContactUsAansweredNotification()
            {
                FullName = FullName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                Subject = Subject,
                Text = Text,
                Reply = Reply,
                Responder = Responder.FullName,
                ReplyDateTime = ReplyDateTime.Value
            });
        }
    }
}
