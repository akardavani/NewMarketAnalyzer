using System.Collections.Generic;

namespace MarketAnalyzer.Infrastructure.Messaging.Asiatech
{
    internal class AsiatechSendResponse
    {
        public string Message { get; set; }
        public bool Succeeded { get; set; }
        public List<string> Data { get; set; }
        public int ResultCode { get; set; }
    }
}
