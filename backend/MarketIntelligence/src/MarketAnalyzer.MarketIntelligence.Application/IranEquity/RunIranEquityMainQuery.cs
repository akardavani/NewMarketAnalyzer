using MarketAnalyzer.Abstractions;
using MarketAnalyzer.MarketIntelligence.Application.IranEquity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketAnalyzer.MarketIntelligence.Application.IranEquity
{
    public class RunIranEquityMainQuery : IRequest<ApiResult<IranEquityMainResultDto>>
    {
        public DateTime? TargetDate { get; set; }
        public string? InputFilePath { get; set; }
    }
}
