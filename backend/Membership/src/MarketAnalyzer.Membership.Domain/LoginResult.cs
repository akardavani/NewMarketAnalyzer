using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Domain
{
    public class LoginResult : Enumeration
    {
        public static readonly LoginResult Success = new(1, "موفق");
        public static readonly LoginResult UserNotFund = new(2, "کاربر پیدا نشد");
        public static readonly LoginResult Deactivated = new(3, "کاربر غیرفعال");
        public static readonly LoginResult LockedOut = new(4, "کاربر مسدود شده");
        public static readonly LoginResult InvalidPassword = new(5, "رمز اشتباه");
        public static readonly LoginResult InvalidOtp = new(6, "رمز دوعاملی اشتباه");
        public static readonly LoginResult RequiredTfa = new(7, "نیازمند رمز عبور دوعاملی");
        public static readonly LoginResult TooManyOtpRequests = new(8, "درخواست بیش از حد رمز عبور دوعاملی");

        private LoginResult(int id, string name) : base(id, name)
        {
        }
    }
}
