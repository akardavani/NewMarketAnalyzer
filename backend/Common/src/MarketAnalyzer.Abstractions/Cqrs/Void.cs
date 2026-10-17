using System.Diagnostics.CodeAnalysis;
using System;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public readonly struct Void : IEquatable<Void>, IComparable<Void>, IComparable
    {
        public static readonly Void Value = new();

        public static readonly Task<Void> Task = System.Threading.Tasks.Task.FromResult(Value);

        public int CompareTo([AllowNull] Void other)
        {
            return 0;
        }

        public int CompareTo(object obj)
        {
            return 0;
        }

        public bool Equals([AllowNull] Void other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Void left, Void right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Void left, Void right)
        {
            return !(left == right);
        }

        public static bool operator <(Void left, Void right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Void left, Void right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Void left, Void right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Void left, Void right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}