using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;

namespace MarketAnalyzer.Membership.Domain
{
    public class Role : AggregateRoot<int>, IConcurrencyEntity
    {
        public Role(int id) : base(id)
        {

        }

        #region [Properties]

        /// <summary>
        /// نام سیستمی
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// نان نمایشی
        /// </summary>
        public string DisplayName { get; private set; }
        /// <summary>
        /// فعال
        /// </summary>
        public bool IsActive { get; private set; }
        /// <summary>
        /// قابل ویرایش
        /// </summary>
        public bool Editable { get; private set; }
        /// <summary>
        /// نسخه رکورد
        /// </summary>
        public byte[] RowVersion { get; private set; }

        private List<User> _users;
        /// <summary>
        /// کاربران
        /// </summary>
        public IReadOnlyCollection<User> Users => _users ??= [];

        private List<Route> _authorizedRoutes;
        /// <summary>
        /// دسترسی‌ها
        /// </summary>
        public IReadOnlyCollection<Route> AuthorizedRoutes => _authorizedRoutes ??= [];

        #endregion

        public static Role Create(string name, string displayName, bool isActive, bool editable)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            if (string.IsNullOrEmpty(displayName))
            {
                throw new ArgumentException($"'{nameof(displayName)}' cannot be null or empty.", nameof(displayName));
            }

            return new Role(0)
            {
                Name = name,
                DisplayName = displayName,
                IsActive = isActive,
                Editable = editable
            };
        }

        public void Update(string name, string displayName, bool isActive)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            if (string.IsNullOrEmpty(displayName))
            {
                throw new ArgumentException($"'{nameof(displayName)}' cannot be null or empty.", nameof(displayName));
            }

            Name = name;
            DisplayName = displayName;
            IsActive = isActive;
        }
    }
}