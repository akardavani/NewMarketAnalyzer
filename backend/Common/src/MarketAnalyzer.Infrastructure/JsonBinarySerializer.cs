using System;
using System.Text.Json;
using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Infrastructure
{
    internal class JsonBinarySerializer : IBinarySerializer
    {
        public JsonBinarySerializer()
        {

        }

        public byte[] Serialize<T>(T obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }

        public T Deserialize<T>(ReadOnlyMemory<byte> buffer)
        {
            if (typeof(T) == typeof(byte[]))
                return (T)(object)buffer.ToArray();

            return JsonSerializer.Deserialize<T>(buffer.Span);
        }
    }
}
