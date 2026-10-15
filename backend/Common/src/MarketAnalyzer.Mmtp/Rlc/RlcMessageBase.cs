namespace MarketAnalyzer.Mmtp
{
    public abstract class RlcMessageBase
    {
        public RlcMessageBase(byte[] buffer)
        {
            Header = new RlcHeader(buffer);
            HeaderLength = 83;
        }

        protected int HeaderLength { get; }
        public RlcHeader Header { get; }
    }
}