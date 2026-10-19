namespace MarketAnalyzer.MarketIntelligence.Domain
{
    public class CryptoSymbol 
    {
        public CryptoSymbol()
        {

        }

        public CryptoSymbol(BaseKucoinCryptoSymbol baseKucoinSymbol)
        {
            Id = baseKucoinSymbol.Id;
            Symbol = baseKucoinSymbol.Symbol;
            SymbolCode = baseKucoinSymbol.SymbolCode;
            Vol = StingToDecimal(baseKucoinSymbol.Vol);
            VolValue = StingToDecimal(baseKucoinSymbol.VolValue);
            LastTradedPrice = Math.Round(Convert.ToDecimal(baseKucoinSymbol.LastTradedPrice), 15);
            BaseCurrency = baseKucoinSymbol.BaseCurrency;
            QuoteCurrency = baseKucoinSymbol.QuoteCurrency;
            Trading = baseKucoinSymbol.Trading;
            Market = baseKucoinSymbol.Market;
        }

        private decimal StingToDecimal(string volString)
        {
            if (Decimal.TryParse(volString, out decimal volDecimal))
            {
                volDecimal = Math.Round(volDecimal, 10);
            }
            return volDecimal;
        }

        public long Id { get; set; }

        public string Symbol { get; set; }

        public string SymbolCode { get; set; }

        public decimal Vol { get; set; }

        public decimal VolValue { get; set; }

        public string BaseCurrency { get; set; }

        public string QuoteCurrency { get; set; }

        public bool Trading { get; set; }

        public string Market { get; set; }

        public decimal LastTradedPrice { get; set; }

    }
}
