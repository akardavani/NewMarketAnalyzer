using System;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.DataProvider.MabnaModels
{
    /// <summary>
    /// اختیار معامله
    /// </summary>
    public class OptionContract : EntityBase
    {
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string ShortName { get; set; }
        public string EnglishShortName { get; set; }
        public Stock Stock { get; set; }
        public string TradeSymbol { get; set; }
        public string EnglishTradeSymbol { get; set; }
        public Exchange Exchange { get; set; }
        public Company PublisherCompany { get; set; }
        public DateTime TradeStartDate { get; set; }
        public DateTime TradeEndDate { get; set; }
        public DateTime ExerciseDate { get; set; }
        public decimal ExercisePrice { get; set; }
        public OptionContractType Type { get; set; }
        public OptionContractStyle Style { get; set; }
        public int Size { get; set; }

        [JsonIgnore]
        public string CompanyId => Stock?.Company?.Id;
    }
}
