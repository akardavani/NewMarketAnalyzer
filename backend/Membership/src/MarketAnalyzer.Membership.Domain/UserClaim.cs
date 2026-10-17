using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;

namespace MarketAnalyzer.Membership.Domain
{
    public class UserClaim : ValueObject
    {
        public UserClaim()
        {

        }

        public string Type { get; private set; }
        public string Value { get; private set; }

        public static UserClaim Create(string type, string value)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException($"'{nameof(type)}' cannot be null or empty.", nameof(type));
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));
            }

            return new UserClaim()
            {
                Type = type,
                Value = value
            };
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Type;
            yield return Value;
        }
    }
}
