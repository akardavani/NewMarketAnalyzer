using System;

namespace MarketAnalyzer.DataProvider.ImeModels
{
    /// <summary>
    /// گروه انبار
    /// </summary>
    public class WarehouseGroup
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CommodityId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PackagingId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UnitId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime ExpireTime { get; set; }
        /// <summary>
        /// ضریب محموله
        /// </summary>
        public decimal CargoCoefficient { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CargoMinAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CargoMaxAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsCargoDividable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEnterReceiptEnable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal IdealPurity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MaxPurity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal MinPurity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public WarehouseGroupCommodity Commodity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Unit Unit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Packaging Packaging { get; set; }
    }
}
