using MarketIntelligence.Domain;

namespace MarketAnalyzer.Collector
{
    public class CandelClientFactory : ICandelClientFactory
    {
        private readonly IDictionary<MarketType, ICandelClient> _candelClients;

        public CandelClientFactory(IEnumerable<ICandelClient> clients)
        {
            // ایجاد دیکشنری از کلاینت‌ها بر اساس MarketType
            try
            {
                _candelClients = clients
                .OfType<ICandelClientWithType>()
                .ToDictionary(c => c.MarketType, c => (ICandelClient)c);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public ICandelClient GetCandelClient(MarketType marketType)
        {
            return _candelClients.TryGetValue(marketType, out var client) ? client : null;
        }        
    }
}
