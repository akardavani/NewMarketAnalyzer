using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.SharedKernel
{
    /// <summary>
    /// NAV رویداد بازگشت به عقب 
    /// </summary>
    public class NetAssetValueRolledBackNotification : INotification
    {
        /// <summary>
        /// NAV تاریخ محاسبه 
        /// </summary>
        public required DateOnly Date { get; init; }
    }
}
