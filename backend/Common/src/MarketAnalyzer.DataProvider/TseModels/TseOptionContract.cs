using System;

namespace MarketAnalyzer.DataProvider.TseModels
{
    public class TseOptionContract
    {
        public long InsCode { get; set; }
        /// <summary>
        /// شناسه دارایی پایه UAInsCode
        /// </summary>
        public long UnderlyingAssetInsCode { get; set; }
        /// <summary>
        /// شناسه نماد InstrumentID
        /// </summary>
        public string InstrumentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long BuyOP { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long SellOP { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long ContractSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal StrikePrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateOnly BeginDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateOnly EndDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AFactor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal BFactor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CFactor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long YesterdayOP { get; set; }
    }
}