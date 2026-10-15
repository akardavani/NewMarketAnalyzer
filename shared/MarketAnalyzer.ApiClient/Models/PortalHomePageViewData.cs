using System;
using System.Collections.Generic;

namespace MarketAnalyzer.ApiClient.Models
{
    public class PortalHomePageViewData
    {
        /// <summary>
        /// اطلاعات سرمایه گذاری
        /// </summary>
        public required InvestmentInformation InvestmentInformation { get; init; }
        /// <summary>
        /// اطلاعات صندوق
        /// </summary>
        public required IReadOnlyDictionary<string, string> FundInformation { get; init; }
        /// <summary>
        /// بنرهای اسلایدر
        /// </summary>
        public required IReadOnlyList<CarsoulItem> CarsoulItems { get; init; }
        /// <summary>
        /// NAV تاریخ آخرین
        /// </summary>
        public required DateOnly LastNavDate { get; init; }
        /// <summary>
        /// تاریخچه NAV
        /// </summary>
        public required NavHistory Navs { get; init; }
        /// <summary>
        /// ترکیب دارایی‌ها
        /// </summary>
        public required IReadOnlyDictionary<string, decimal> AssetsComposition { get; init; }
        /// <summary>
        /// ترکیب دارایی‌ها بر حسب صنایع
        /// </summary>
        public required IReadOnlyDictionary<string, decimal> SectorsAssetsComposition { get; init; }
    }
}
