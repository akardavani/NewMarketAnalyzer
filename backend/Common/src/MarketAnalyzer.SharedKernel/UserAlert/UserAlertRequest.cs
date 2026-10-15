using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;

namespace MarketAnalyzer.SharedKernel
{
    public class UserAlertRequest : IRequest<IReadOnlyCollection<UserAlertResult>>
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public int UserId { get; init; }
        /// <summary>
        /// Nav تاریخ روز بعد 
        /// </summary>
        public required DateOnly NextNavDate { get; init; }
        /// <summary>
        /// تاریخ روز
        /// </summary>
        public required DateOnly CurrentDate { get; init; }
    }
}
