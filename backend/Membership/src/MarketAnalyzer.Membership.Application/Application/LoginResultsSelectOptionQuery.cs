using MarketAnalyzer.Abstractions;
using System.Collections.Generic;

namespace MarketAnalyzer.Membership.Application
{
    public class LoginResultsSelectOptionQuery : IRequest<IReadOnlyCollection<SelectOption>>
    {
    }
}
