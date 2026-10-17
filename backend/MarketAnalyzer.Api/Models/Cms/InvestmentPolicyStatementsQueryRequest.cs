using System;
using System.Linq;

namespace MarketAnalyzer.Api.Models
{
    public class InvestmentPolicyStatementsQueryRequest : Pagination
    {
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        /// <summary>
        /// فعال/غیرفعال
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
