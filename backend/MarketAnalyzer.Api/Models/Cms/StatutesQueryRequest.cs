using System;
using System.Linq;

namespace MarketAnalyzer.Api.Models
{
    public class StatutesQueryRequest : Pagination
    {
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
