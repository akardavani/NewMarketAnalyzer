using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Membership.Domain
{
    public class Route : AggregateRoot<int>
    {
        public Route(int id) : base(id)
        {
        }

        #region [Properties]

        public string Controller { get; private set; }
        public string Action { get; private set; }
        public string Path { get; private set; }
        public string HashCode { get; private set; }
        public bool IsActive { get; private set; }

        private List<Role> _roles;
        public IReadOnlyCollection<Role> AuthorizedRoles => _roles ??= [];

        #endregion

        public static Route Create(
            string controller,
            string action,
            string path,
            string hashCode,
            bool isActive)
        {
            if (string.IsNullOrEmpty(controller))
            {
                throw new ArgumentException($"'{nameof(controller)}' cannot be null or empty.", nameof(controller));
            }

            if (string.IsNullOrEmpty(action))
            {
                throw new ArgumentException($"'{nameof(action)}' cannot be null or empty.", nameof(action));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
            }

            if (string.IsNullOrEmpty(hashCode))
            {
                throw new ArgumentException($"'{nameof(hashCode)}' cannot be null or empty.", nameof(hashCode));
            }

            return new Route(0)
            {
                Controller = controller,
                Action = action,
                Path = path,
                HashCode = hashCode,
                IsActive = isActive
            };
        }

        public void Update(
            string controller,
            string action,
            string path,
            string hashCode,
            bool isActive)
        {
            if (string.IsNullOrEmpty(controller))
            {
                throw new ArgumentException($"'{nameof(controller)}' cannot be null or empty.", nameof(controller));
            }

            if (string.IsNullOrEmpty(action))
            {
                throw new ArgumentException($"'{nameof(action)}' cannot be null or empty.", nameof(action));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
            }

            if (string.IsNullOrEmpty(hashCode))
            {
                throw new ArgumentException($"'{nameof(hashCode)}' cannot be null or empty.", nameof(hashCode));
            }

            Controller = controller;
            Action = action;
            Path = path;
            HashCode = hashCode;
            IsActive = isActive;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void AddRole(Role role)
        {
            ArgumentNullException.ThrowIfNull(role);

            if (!AuthorizedRoles.Contains(role))
            {
                _roles.Add(role);
            }
        }

        public void RemoveRole(Role role)
        {
            if (!AuthorizedRoles.Contains(role))
            {
                throw new Exception("The role not fund in AuthorizedRoles.");
            }

            _roles.Remove(role);
        }
    }
}