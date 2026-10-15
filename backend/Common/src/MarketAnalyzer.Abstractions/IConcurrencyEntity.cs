namespace MarketAnalyzer.Abstractions
{
    public interface IConcurrencyEntity
    {
        public byte[] RowVersion { get; }
    }
}