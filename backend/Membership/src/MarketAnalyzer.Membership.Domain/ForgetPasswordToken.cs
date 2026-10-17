using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.Membership.Domain
{
    public class ForgetPasswordToken : AuditableEntity<long>
    {
        public const string ApplicationId = nameof(ApplicationId);

        public ForgetPasswordToken(long id) : base(id)
        {
        }

        #region [Properties]

        /// <summary>
        /// درخواست کننده IP آدرس
        /// </summary>
        public string IpAddress { get; private set; }
        /// <summary>
        /// زمان انقضا
        /// </summary>
        public DateTime ExpiryDateTime { get; private set; }
        /// <summary>
        /// توکن امنیتی
        /// </summary>
        public string Token { get; private set; }
        /// <summary>
        /// استفاده شده
        /// </summary>
        public bool IsUsed { get; private set; }

        #endregion

        public static ForgetPasswordToken Create(
            string ipAddress,
            DateTime expiryDateTime,
            string token)
        {
            ArgumentNullException.ThrowIfNull(ipAddress);

            return new ForgetPasswordToken(0)
            {
                IpAddress = ipAddress,
                ExpiryDateTime = expiryDateTime,
                Token = token,
                IsUsed = false
            };
        }

        public void Invalidate()
        {
            IsUsed = true;
        }
    }
}