using System;

namespace MarketAnalyzer.DataProvider.ImeModels
{
    public class WarehouseBillTransaction
    {
        public int Id { get; set; }
        public string GroupId { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string ReferenceId { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
