using System;

namespace MarketAnalyzer.SharedKernel
{
    public class AccountValue
    {
        private readonly object _value;

        private AccountValue(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException($"'{nameof(code)}' cannot be null or empty.", nameof(code));
            }

            _value = code;
        }

        private AccountValue(int indicator)
        {
            _value = indicator;
        }

        public bool IsIndicator => _value is int;
        public string ValueAsString => (string)_value;
        public int ValueAsIndicator => (int)_value;

        public override bool Equals(object obj)
        {
            if (obj is not AccountValue other)
            {
                return false;
            }

            return other._value.Equals(_value);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return (_value is string stringValue) ?
                stringValue :
                _value.ToString();
        }

        public static bool operator ==(AccountValue account, AccountValue other) => account.Equals(other);
        public static bool operator !=(AccountValue account, AccountValue other) => !account.Equals(other);


        public static implicit operator AccountValue(AccountIndicator indicator)
        {
            if (indicator is null)
            {
                throw new ArgumentNullException(nameof(indicator));
            }

            return new AccountValue(indicator.Id);
        }

        public static implicit operator AccountValue(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException($"'{nameof(code)}' cannot be null or empty.", nameof(code));
            }

            return new(code);
        }

        public static implicit operator AccountValue(int indicator) => new(indicator);
    }
}