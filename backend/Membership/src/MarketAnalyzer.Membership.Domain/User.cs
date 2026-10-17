using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain.Notifications;
using MarketAnalyzer.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Membership.Domain
{
    public class User : AggregateRoot<int>
    {
        public const string ApplicationId = nameof(ApplicationId);

        public User(int id) : base(id)
        {
        }

        #region [Properties]

        /// <summary>
        /// نام و نام خانوادگی
        /// </summary>
        public string FullName { get; private set; }
        /// <summary>
        /// نام کاربری
        /// </summary>
        public string UserName { get; private set; }
        /// <summary>
        /// رمز عبور هش شده
        /// </summary>
        public string Password { get; private set; }
        /// <summary>
        /// شماره موبایل
        /// </summary>
        public string PhoneNumber { get; private set; }
        /// <summary>
        /// آدرس ایمیل
        /// </summary>
        public string Email { get; private set; }
        /// <summary>
        /// ایمیل معتبر
        /// </summary>
        public bool EmailConfirmed { get; private set; }
        /// <summary>
        /// ورود دو عاملی
        /// </summary>
        public bool TwoFactorAuthentication { get; private set; }
        /// <summary>
        /// کاربر فعال
        /// </summary>
        public bool IsActive { get; private set; }
        /// <summary>
        /// آخرین ورود
        /// </summary>
        public DateTime? LastLoginDate { get; private set; }
        /// <summary>
        /// شناسه امنیتی
        /// </summary>
        public byte[] SecurityStamp { get; private set; }
        /// <summary>
        /// برنامه کاربردی
        /// </summary>
        public Application Application { get; private set; }

        private List<Role> _roles;
        /// <summary>
        /// نقش ها
        /// </summary>
        public IReadOnlyCollection<Role> Roles => _roles ??= [];

        private List<UserClaim> _claims;
        /// <summary>
        /// شناسه های هویت
        /// </summary>
        public IReadOnlyCollection<UserClaim> Claims => _claims ??= [];

        private List<IPString> _ipAddresses;
        /// <summary>
        /// IP آدرس‌های
        /// </summary>
        public IReadOnlyCollection<IPString> IpAddresses => _ipAddresses ??= [];

        private List<LoginHistory> _loginHistories;
        /// <summary>
        /// تاریخچه ورود و خروج
        /// </summary>
        public IReadOnlyCollection<LoginHistory> LoginHistories => _loginHistories ??= [];

        private List<ForgetPasswordToken> _forgetPasswordTokens;
        /// <summary>
        /// توکن‌های بازیابی رمز عبور
        /// </summary>
        public IReadOnlyCollection<ForgetPasswordToken> ForgetPasswordTokens => _forgetPasswordTokens ??= [];

        #endregion

        public static User Create(
            string fullName,
            string userName,
            string password,
            string phoneNumber,
            string email,
            bool emailConfirmed,
            bool twoFactorAuthentication,
            Application application,
            IPasswordHasher passwordHasher)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException($"'{nameof(fullName)}' cannot be null or empty.", nameof(fullName));
            }

            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException($"'{nameof(userName)}' cannot be null or empty.", nameof(userName));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));
            }

            if (string.IsNullOrEmpty(phoneNumber))
            {
                throw new ArgumentException($"'{nameof(phoneNumber)}' cannot be null or empty.", nameof(phoneNumber));
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException($"'{nameof(email)}' cannot be null or empty.", nameof(email));
            }

            ArgumentNullException.ThrowIfNull(application);
            ArgumentNullException.ThrowIfNull(passwordHasher);

            var user = new User(0)
            {
                FullName = fullName,
                UserName = userName.ToLower(),
                Password = passwordHasher.HashPassword(password),
                PhoneNumber = phoneNumber,
                Email = email?.ToLower(),
                Application = application,
                TwoFactorAuthentication = twoFactorAuthentication,
                EmailConfirmed = !string.IsNullOrEmpty(email) && emailConfirmed,
                IsActive = true
            };

            user.UpdateSecurityStamp();

            user.AddDomainEvent(() => new UserCreatedNotification()
            {
                UserId = user.Id,
                UserName = userName,
                FullName = fullName,
                Password = password,
                Email = email,
                EmailConfirmed = emailConfirmed,
                PhoneNumber = phoneNumber,
            });

            return user;
        }

        public void Update(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException($"'{nameof(fullName)}' cannot be null or empty.", nameof(fullName));
            }

            FullName = fullName;
        }

        public void UpdateUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException($"'{nameof(userName)}' cannot be null or empty.", nameof(userName));
            }

            if (!UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
            {
                UserName = userName.ToLower();
                UpdateSecurityStamp();
            }
        }

        /// <summary>
        /// تغییر رمز عبور کاربر
        /// </summary>
        /// <param name="password">پسورد هش نده</param>
        /// <param name="raiseEvent">رویداد تغییر رمزعبور منتشر شود یا نه، اگر منتشر شود رمزعبور از طریق پیامک برای کاربر ارسال خواهد شد</param>
        /// <param name="passwordHasher">هش کننده پسورد</param>
        public void UpdatePassword(string password, bool raiseEvent, IPasswordHasher passwordHasher)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));
            }

            ArgumentNullException.ThrowIfNull(passwordHasher);

            var hashedPassword = passwordHasher.HashPassword(password);

            if (Password != hashedPassword)
            {
                Password = hashedPassword;
                UpdateSecurityStamp();
            }

            if (raiseEvent)
            {
                AddDomainEvent(() => new UserPasswordResetedNotification()
                {
                    UserId = Id,
                    Email = Email,
                    UserName = UserName,
                    FullName = FullName,
                    Password = password,
                    PhoneNumber = PhoneNumber,
                    EmailConfirmed = EmailConfirmed,
                });
            }
        }

        public void UpdatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                throw new ArgumentException($"'{nameof(phoneNumber)}' cannot be null or empty.", nameof(phoneNumber));
            }

            if (PhoneNumber != phoneNumber)
            {
                PhoneNumber = phoneNumber;
                UpdateSecurityStamp();
            }
        }

        public void UpdateEmail(string email, bool confirmed)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException($"'{nameof(email)}' cannot be null or empty.", nameof(email));
            }

            if (Email != email?.ToLower() || EmailConfirmed != confirmed)
            {
                Email = email?.ToLower();
                EmailConfirmed = confirmed;
                UpdateSecurityStamp();
            }
        }

        public void UpdateLastLoginDate(DateTime lastLoginDate)
        {
            LastLoginDate = lastLoginDate;
        }

        public void Activate()
        {
            IsActive = true;
            UpdateSecurityStamp();
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateSecurityStamp();
        }

        public void AddIpAddress(IPString ip)
        {
            if (!IpAddresses.Contains(ip))
            {
                _ipAddresses.Add(ip);
                UpdateSecurityStamp();
            }
        }

        public void RemoveIpAddress(IPString ip)
        {
            if (!IpAddresses.Contains(ip))
            {
                throw new Exception("The ip address not fund in IpAddresses.");
            }

            _ipAddresses.Remove(ip);
            UpdateSecurityStamp();
        }

        public void AddClaim(string key, string value)
        {
            var claim = UserClaim.Create(key, value);
            if (!Claims.Contains(claim))
            {
                _claims.Add(claim);
            }
        }

        public void AddRole(Role role)
        {
            ArgumentNullException.ThrowIfNull(role);

            if (!Roles.Contains(role))
            {
                _roles.Add(role);
                UpdateSecurityStamp();
            }
        }

        public void RemoveRole(Role role)
        {
            ArgumentNullException.ThrowIfNull(role);

            if (!Roles.Contains(role))
            {
                throw new Exception("The role not fund in Roles.");
            }

            _roles.Remove(role);
            UpdateSecurityStamp();
        }

        public void AddLoginHistory(LoginHistory loginHistory)
        {
            if (!LoginHistories.Contains(loginHistory))
            {
                _loginHistories.Add(loginHistory);
            }
        }

        public void AddForgetPasswordToken(ForgetPasswordToken token)
        {
            if (!ForgetPasswordTokens.Contains(token))
            {
                _forgetPasswordTokens.Add(token);
            }
        }

        private void UpdateSecurityStamp()
        {
            SecurityStamp = Guid.NewGuid().ToByteArray();
        }
    }
}