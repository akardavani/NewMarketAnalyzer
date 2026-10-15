using System;

namespace MarketAnalyzer.DataProvider.ImeModels
{
    public class SettlementPrice
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string PersianDate { get; set; }
        public decimal Price { get; set; }

        public DateOnly Date => DateOnly.Parse(PersianDate, CaltureHelper.FaCulture);
    }
}
