using System.Collections.Generic;

namespace MarketAnalyzer.Abstractions.Routes
{
    public interface IRouteDescriptorProvider
    {
        IEnumerable<RouteDescriptor> GetRouteDescriptors();
    }
}
