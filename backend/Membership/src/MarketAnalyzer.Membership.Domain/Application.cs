using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.Membership.Domain
{
    public class Application : AggregateRoot<int>
    {
        public static readonly Application AdminWeb = Create(
            id: 1,
            displayName: "پنل مدیریت",
            applicationName: "Admin Web",
            applicationKey: "d9eef0c781404a1d98a55a16a3de31e9",
            isActive: true,
            sessionLifetime: TimeSpan.FromMinutes(720),
            otpTokenLifetime: TimeSpan.FromMinutes(2),
            changePasswordTokenLifetime: TimeSpan.FromMinutes(10));

        public static readonly Application ClientWeb = Create(
            id: 2,
            displayName: "پورتال",
            applicationName: "Client Portal",
            applicationKey: "3aa5ef65ba194ca6bd427016a7d966a3",
            isActive: true,
            sessionLifetime: TimeSpan.FromMinutes(720),
            otpTokenLifetime: TimeSpan.FromMinutes(2),
            changePasswordTokenLifetime: TimeSpan.FromMinutes(10));

        public Application(int id) : base(id)
        {
        }

        #region [Properties]

        /// <summary>
        /// نام نمایشی
        /// </summary>
        public string DisplayName { get; private set; }
        /// <summary>
        /// اسم برنامه
        /// </summary>
        public string ApplicationName { get; private set; }
        /// <summary>
        /// API کلید 
        /// </summary>
        public string ApplicationKey { get; private set; }
        /// <summary>
        /// برنامه فعال
        /// </summary>
        public bool IsActive { get; private set; }
        /// <summary>
        /// طول عمر جلسه
        /// </summary>
        public TimeSpan SessionLifetime { get; private set; }
        /// <summary>
        /// طول عمر رمز یکبار مصرف
        /// </summary>
        public TimeSpan OtpTokenLifetime { get; private set; }
        /// <summary>
        /// طول عمر کد تغییر رمز عبور
        /// </summary>
        public TimeSpan ChangePasswordTokenLifetime { get; private set; }

        #endregion

        public static Application Create(
            int id,
            string displayName,
            string applicationName,
            string applicationKey,
            bool isActive,
            TimeSpan sessionLifetime,
            TimeSpan otpTokenLifetime,
            TimeSpan changePasswordTokenLifetime)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                throw new ArgumentException($"'{nameof(displayName)}' cannot be null or empty.", nameof(displayName));
            }

            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentException($"'{nameof(applicationName)}' cannot be null or empty.", nameof(applicationName));
            }

            if (string.IsNullOrEmpty(applicationKey))
            {
                throw new ArgumentException($"'{nameof(applicationKey)}' cannot be null or empty.", nameof(applicationKey));
            }

            return new Application(id)
            {
                DisplayName = displayName,
                ApplicationName = applicationName,
                ApplicationKey = applicationKey,
                IsActive = isActive,
                SessionLifetime = sessionLifetime,
                OtpTokenLifetime = otpTokenLifetime,
                ChangePasswordTokenLifetime = changePasswordTokenLifetime
            };
        }

        public void Update(
            string displayName,
            string applicationName,
            string applicationKey,
            bool isActive,
            TimeSpan sessionLifetime,
            TimeSpan otpTokenLifetime,
            TimeSpan changePasswordTokenLifetime)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                throw new ArgumentException($"'{nameof(displayName)}' cannot be null or empty.", nameof(displayName));
            }

            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentException($"'{nameof(applicationName)}' cannot be null or empty.", nameof(applicationName));
            }

            if (string.IsNullOrEmpty(applicationKey))
            {
                throw new ArgumentException($"'{nameof(applicationKey)}' cannot be null or empty.", nameof(applicationKey));
            }

            DisplayName = displayName;
            ApplicationName = applicationName;
            ApplicationKey = applicationKey;
            IsActive = isActive;
            SessionLifetime = sessionLifetime;
            OtpTokenLifetime = otpTokenLifetime;
            ChangePasswordTokenLifetime = changePasswordTokenLifetime;
        }
    }
}