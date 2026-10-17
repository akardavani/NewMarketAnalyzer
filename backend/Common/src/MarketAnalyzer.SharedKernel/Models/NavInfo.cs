using System;

namespace MarketAnalyzer.SharedKernel
{
    /// <summary>
    /// NAV اطلاعات
    /// </summary>
    public class NavInfo
    {
        /// <summary>
        /// تاریخ
        /// </summary>
        public required DateOnly Date { get; init; }
        /// <summary>
        /// صدور NAV
        /// </summary>
        public required decimal IssuanceNav { get; init; }
        /// <summary>
        /// ابطال NAV
        /// </summary>
        public required decimal CancellationNav { get; init; }
        /// <summary>
        /// آماری NAV
        /// </summary>
        public required decimal StatisticalNav { get; init; }
        /// <summary>
        /// تعداد گواهی های عادی
        /// </summary>
        public required long OrdineryUnitCertificate { get; init; }
        /// <summary>
        /// تعداد گواهی های ممتاز
        /// </summary>
        public required long SuperUnitCertificate { get; init; }
    }
}
