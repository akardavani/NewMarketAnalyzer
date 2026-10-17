using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.Membership.Domain
{
    public class LoginHistory : AuditableEntity<long>
    {
        public const string LoginResultId = nameof(LoginResultId);

        public LoginHistory(long id) : base(id)
        {

        }

        public LoginType LoginType { get; private set; }
        public string IpAddress { get; private set; }
        public LoginResult LoginResult { get; private set; }
        public Application Application { get; private set; }
        public string UserAgent { get; private set; }

        public static LoginHistory Create(
            LoginType loginType,
            string ipAddress,
            LoginResult loginResult,
            Application application,
            string userAgent)
        {
            if (ipAddress is null)
            {
                throw new ArgumentNullException(nameof(ipAddress));
            }

            if (loginResult is null)
            {
                throw new ArgumentNullException(nameof(loginResult));
            }

            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (string.IsNullOrEmpty(userAgent))
            {
                throw new ArgumentException($"'{nameof(userAgent)}' cannot be null or empty.", nameof(userAgent));
            }

            return new LoginHistory(0)
            {
                LoginType = loginType,
                IpAddress = ipAddress,
                LoginResult = loginResult,
                Application = application,
                UserAgent = userAgent
            };
        }
    }
}