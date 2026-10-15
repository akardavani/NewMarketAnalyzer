using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application
{
    public class RemoveNewsCategoryCommand : IRequest<ApiResult<int>>
    {
        public required int Id { get; init; }
        public required int OperatorId { get; init; }
    }
}
