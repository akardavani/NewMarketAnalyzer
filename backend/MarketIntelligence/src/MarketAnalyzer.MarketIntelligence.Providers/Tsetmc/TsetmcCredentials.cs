using MarketAnalyzer.MarketIntelligence.Domain;
using Microsoft.Extensions.Options;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// اعتبارنامهٔ وب‌سرویس TSETMC. اولویت: Options (User Secrets / Environment)، بعد فایل قدیمی url_settings.json (پل سازگاری تا فاز ۷).
    /// رمز هرگز در دیتابیس یا لاگ ذخیره نمی‌شود.
    /// </summary>
    public sealed class TsetmcCredentials
    {
        /// <summary>سازنده.</summary>
        public TsetmcCredentials(IOptions<TsetmcOptions> options, IOptions<UrlSettings> legacy)
        {
            var legacyService = legacy.Value?.TseTmcWebServiceSettings;
            User = FirstNonEmpty(options.Value.User, legacyService?.User);
            Password = FirstNonEmpty(options.Value.Password, legacyService?.Password);
        }

        /// <summary>نام کاربری؛ تهی یعنی تنظیم نشده.</summary>
        public string? User { get; }

        /// <summary>رمز؛ تهی یعنی تنظیم نشده.</summary>
        public string? Password { get; }

        private static string? FirstNonEmpty(params string?[] values) => values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
    }
}
