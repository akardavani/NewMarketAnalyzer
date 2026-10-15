using System;

namespace MarketAnalyzer.Abstractions
{
    public interface IBinarySerializer
    {
        byte[] Serialize<T>(T obj);
        T Deserialize<T>(ReadOnlyMemory<byte> buffer);
    }
}
