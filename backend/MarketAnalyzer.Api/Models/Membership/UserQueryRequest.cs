using System.Linq;

namespace MarketAnalyzer.Api.Models
{
    public class UserQueryRequest : Pagination
    {
        /// <summary>
        /// نام و نام خانوادگی
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// نام کاربری
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// شماره موبایل
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// ایمیل
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// ورود دو عاملی
        /// </summary>
        public bool?  TwoFactorAuthentication { get; set; }
        /// <summary>
        /// فعال/غیر فعال
        /// </summary>
        public bool? IsActive { get; set; }
    }
}