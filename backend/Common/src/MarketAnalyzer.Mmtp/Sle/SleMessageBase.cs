namespace MarketAnalyzer.Mmtp
{
    public class SleMessageBase
    {
        public SleMessageBase(byte[] buffer)
        {
            BrokerId = MmtpConvert.GetString(buffer, 0, 3);
            GapName = MmtpConvert.GetString(buffer, 3, 2);
            RequestId = MmtpConvert.GetInt64(buffer, 5, 11);
            SleMessageType = MmtpConvert.GetString(buffer, 16, 4);
        }

        public string BrokerId { get; }
        public string GapName { get; }
        public long RequestId { get; }
        public string SleMessageType { get; }
    }
}
