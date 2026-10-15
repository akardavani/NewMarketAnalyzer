using System;
using System.Collections.Generic;

namespace MarketAnalyzer.Api.Models
{
    public class CreateUserRequest
    {
        /// <summary>
        /// نام
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// نام کاربری
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// رمزعبور
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// تکرار رمز عبور
        /// </summary>
        public string ConfirmPassword { get; set; }
        /// <summary>
        /// شماره موبایل
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// آدرس ایمیل
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// ورود دو عاملی
        /// </summary>
        public bool TwoFactorAuthentication { get; set; }
        /// <summary>
        /// IP آدرسهای
        /// </summary>
        public string IpAddresses { get; set; }
        /// <summary>
        /// نقش‌ها
        /// </summary>
        public IReadOnlyCollection<int> Roles { get; set; }

        /// <summary>
        /// IP لیست آدرس‌های
        /// </summary>
        public IReadOnlyCollection<string> IpAddressesList
        {
            get
            {
                if (string.IsNullOrEmpty(IpAddresses))
                {
                    return null;
                }

                return IpAddresses.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }
        }
    }
}
