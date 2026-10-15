using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Application.IranEquity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketAnalyzer.Application.IranEquity
{
    public class RunIranEquityMainQuery : IRequest<ApiResult<IranEquityMainResultDto>>
    {
        public DateTime? TargetDate { get; set; }
        public string? InputFilePath { get; set; }
    }
}
