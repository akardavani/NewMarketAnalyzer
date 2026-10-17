

namespace MarketAnalyzer.Domain
{
    public class CryptoTelegramParameterModel
    {
        public List<SignalModel> Signals { get; set; }
        public Timeframe Timeframe { get; set; }
        //public BrokerEnum Broker { get; set; }
        public MarketType Market { get; set; }
        public DateTime Date { get; set; }

    }
}
