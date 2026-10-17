using System.Collections.Generic;

namespace MarketAnalyzer.DataProvider.ImeModels
{
    public class ImeResponse<T>
    {
        /// <summary>
        /// تعداد سطر در صفحه - حداقل 10 و حداکثر 1000
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// تعداد کل رکوردها
        /// </summary>
        public int TotalItemCount { get; set; }
        /// <summary>
        /// شماره صفحه جاری
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// تعداد کل صفحات
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// رکوردها
        /// </summary>
        public List<ItemInfo<T>> Items { get; set; }
    }
}
