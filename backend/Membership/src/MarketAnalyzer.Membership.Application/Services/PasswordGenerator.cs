using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarketAnalyzer.Membership.Application.Services
{
    internal class PasswordGenerator
    {
        private readonly IReadOnlyList<IReadOnlyList<string>> _collections;
        private readonly IReadOnlyList<IReadOnlyList<string>> _alphanumeric;
        private readonly IReadOnlyList<string> _numbers = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        private readonly IReadOnlyList<string> _specials = new List<string>() { "!", "@", "#", "$", "%", "&", "*", "-", "_", "+", "=", ".", "?" };
        private readonly IReadOnlyList<string> _upperCase = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        private readonly IReadOnlyList<string> _lowerCase = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        public PasswordGenerator()
        {
            _collections = new List<IReadOnlyList<string>>()
            {
                _upperCase,
                _lowerCase,
                _specials,
                _numbers
            };

            _alphanumeric = new List<IReadOnlyList<string>>()
            {
                _upperCase,
                _lowerCase,
                _numbers
            };
        }

        public string Generate(int length)
        {
            var rnd = new Random();
            var password = new StringBuilder();
            var indices = RandomIndices(rnd, Math.Min(length, 4));

            for (var i = 0; i < length; i++)
            {
                var filled = indices.FirstOrDefault(e => e.Index == i);
                if (filled is not null)
                {
                    password.Append(filled.Value);
                }
                else
                {
                    var collection = rnd.Next(_alphanumeric);
                    password.Append(rnd.Next(collection));
                }
            }

            return password.ToString();
        }

        private RandomCollection[] RandomIndices(Random rnd, int length)
        {
            var i = 0;
            var indices = new RandomCollection[length];
            while (i < length)
            {
                var index = rnd.Next(0, length);
                var collection = rnd.Next(_collections);
                if (!indices.Where(e => e is not null).Any(e => e.Index == index || e.Collection == collection))
                {
                    indices[i] = new RandomCollection()
                    {
                        Index = index,
                        Collection = collection,
                        Value = rnd.Next(collection)
                    };
                    i++;
                }
            }

            return indices;
        }

        private class RandomCollection
        {
            public required int Index { get; init; }
            public required string Value { get; init; }
            public required IReadOnlyList<string> Collection { get; init; }
        }
    }
}