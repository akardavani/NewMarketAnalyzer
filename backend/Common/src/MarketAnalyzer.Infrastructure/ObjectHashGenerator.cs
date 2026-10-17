using MarketAnalyzer.Abstractions;
using System;
using System.Security.Cryptography;

namespace MarketAnalyzer.Infrastructure
{
    internal class ObjectHashGenerator : IObjectHashGenerator
    {
        private readonly IBinarySerializer _serializer;

        public ObjectHashGenerator(IBinarySerializer serializer)
        {
            _serializer = serializer;
        }

        public byte[] ComputeHash<T>(T obj)
        {
            var buffer = _serializer.Serialize(obj);
            return SHA256.HashData(buffer);
        }

        public string ComputeHashString<T>(T obj)
        {
            var hash = ComputeHash(obj);
            return Convert.ToHexString(hash);
        }
    }
}
