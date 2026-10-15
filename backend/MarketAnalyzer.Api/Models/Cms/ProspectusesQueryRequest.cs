using System;
using System.Linq;

namespace MarketAnalyzer.Api.Models
{
    public class ProspectusesQueryRequest : Pagination
    {
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
