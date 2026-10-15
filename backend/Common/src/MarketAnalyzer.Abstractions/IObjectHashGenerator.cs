namespace MarketAnalyzer.Abstractions
{
    public interface IObjectHashGenerator
    {
        byte[] ComputeHash<T>(T obj);
        string ComputeHashString<T>(T obj);
    }
}
