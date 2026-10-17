using System;

namespace MarketAnalyzer.Mmtp
{
    public class RlcHeader
    {
        public RlcHeader(byte[] buffer)
        {
            MessageTypeCode = $"00{MmtpConvert.GetString(buffer, 22, 2)}";
            InstrumentId = MmtpConvert.GetString(buffer, 55, 12);
            InstrumentGroupCode = MmtpConvert.GetString(buffer, 20, 2);
            InstrumentMnemonic = MmtpConvert.GetString(buffer, 30, 5);
            EventDateTime = MmtpConvert.GetDateTime(buffer, 38, 14);
        }

        public string InstrumentId { get; }
        public string MarketSegment { get; }
        public string InstrumentGroupCode { get; }
        public string InstrumentMnemonic { get; }
        public DateTime EventDateTime { get; }
        public string MessageTypeCode { get; set; }
    }
}