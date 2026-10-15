using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.Membership.Domain
{
    public class UnknownLoginHistory : AggregateRoot<long>
    {
        public UnknownLoginHistory(long id) : base(id)
        {
        }

        public string UserName { get; private set; }
        public string Password { get; private set; }
        public LoginType LoginType { get; private set; }
        public string IpAddress { get; private set; }
        public string UserAgent { get; private set; }
        public Application Application { get; private set; }

        public static UnknownLoginHistory Create(
            string userName,
            string password,
            LoginType loginType,
            string ipAddress,
            string userAgent,
            Application application)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException($"'{nameof(userName)}' cannot be null or empty.", nameof(userName));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));
            }

            if (string.IsNullOrEmpty(ipAddress))
            {
                throw new ArgumentException($"'{nameof(ipAddress)}' cannot be null or empty.", nameof(ipAddress));
            }

            if (string.IsNullOrEmpty(userAgent))
            {
                throw new ArgumentException($"'{nameof(userAgent)}' cannot be null or empty.", nameof(userAgent));
            }

            return new UnknownLoginHistory(0)
            {
                UserName = userName,
                Password = password,
                LoginType = loginType,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Application = application
            };
        }
    }
}
