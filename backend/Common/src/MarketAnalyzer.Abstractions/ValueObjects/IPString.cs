using System;
using System.Collections.Generic;
using System.Net;

namespace MarketAnalyzer.Abstractions
{
    public sealed class IPString : ValueObject
    {
        public IPString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));
            }

            if (!IPAddress.TryParse(value, out var _))
            {
                throw new Exception("Invalid ip address.");
            }

            Value = value;
        }

        public string Value { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
