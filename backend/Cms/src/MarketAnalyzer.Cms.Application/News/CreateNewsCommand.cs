using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    public class CreateNewsCommand : IRequest<ApiResult<int>>
    {
        public required string Title { get; init; }
        public string Source { get; init; }
        public required string Summary { get; init; }
        public required string Text { get; init; }
        public bool IsActive { get; init; }
        public required int OperatorId { get; init; }
    }
}
