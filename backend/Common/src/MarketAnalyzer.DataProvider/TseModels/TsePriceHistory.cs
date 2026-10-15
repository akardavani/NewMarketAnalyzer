using System;

namespace MarketAnalyzer.DataProvider.TseModels
{
    public class TsePriceHistory
    {
        /// <summary>
        /// InsCode
        /// </summary>
        public long InsCode { get; set; }
        /// <summary>
        /// Index[0], DEven
        /// </summary>
        public DateOnly EntryDate { get; set; }
        /// <summary>
        /// Index[5], PriceFirst
        /// </summary>
        public decimal FirstTradePrice { get; set; }
        /// <summary>
        /// Index[1], PriceMax
        /// </summary>
        public decimal HighestTradePrice { get; set; }
        /// <summary>
        /// Index[2], PriceMin
        /// </summary>
        public decimal LowestTradePrice { get; set; }
        /// <summary>
        /// Index[4], PDrCotVal
        /// </summary>
        public decimal LastTradePrice { get; set; }
        /// <summary>
        /// Index[3], PClosing
        /// </summary>
        public decimal ClosingPrice { get; set; }
        /// <summary>
        /// Index[9], ZTotTran
        /// </summary>
        public long TotalTradesNumber { get; set; }
        /// <summary>
        /// Index[7], QTotCap
        /// </summary>
        public decimal TotalTradeValue { get; set; }
        /// <summary>
        /// Index[8], QTotTran5J
        /// </summary>
        public long TotalTradedQuantity { get; set; }
        /// <summary>
        /// Index[6], PriceYesterday
        /// </summary>
        public decimal LastDayClosingPrice { get; set; }
    }
}
