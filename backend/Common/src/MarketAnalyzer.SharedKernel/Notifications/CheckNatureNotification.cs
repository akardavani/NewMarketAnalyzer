using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.SharedKernel.Notifications
{
    /// <summary>
    /// رویداد چک کردن ماهیت حساب
    /// </summary>
    public class CheckNatureNotification : INotification
    {
        /// <summary>
        /// Nav تاریخ 
        /// </summary>
        public required DateOnly NavDate { get; init; }
    }
}
