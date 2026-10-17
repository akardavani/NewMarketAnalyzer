using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MarketAnalyzer.Abstractions.Routes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Infrastructure.Routes
{
    internal class RouteDescriptorProvider : IRouteDescriptorProvider
    {
        private readonly Lazy<IEnumerable<RouteDescriptor>> _routes;
        private readonly IActionDescriptorCollectionProvider _descriptorCollectionProvider;

        public RouteDescriptorProvider(IActionDescriptorCollectionProvider descriptorCollectionProvider)
        {
            _descriptorCollectionProvider = descriptorCollectionProvider;
            _routes = new Lazy<IEnumerable<RouteDescriptor>>(FetchRouteDescriptor, true);
        }

        public IEnumerable<RouteDescriptor> GetRouteDescriptors() =>
            _routes.Value;

        private IEnumerable<RouteDescriptor> FetchRouteDescriptor()
        {
            return _descriptorCollectionProvider
                .ActionDescriptors
                .Items
                .OfType<ControllerActionDescriptor>()
                .Where(e => !IsIgnored(e))
                .GroupBy(route => $"{route.ControllerName}:{route.ActionName}")
                .SelectMany(CreateRouteDescriptor);
        }

        private IEnumerable<RouteDescriptor> CreateRouteDescriptor(IGrouping<string, ControllerActionDescriptor> actionDescriptors)
        {
            foreach (var actionDescriptor in actionDescriptors.GroupBy(x => x.ActionName).Select(GetCondidate))
            {
                yield return new RouteDescriptor(actionDescriptor);
            }
        }

        private ControllerActionDescriptor GetCondidate(IGrouping<string, ControllerActionDescriptor> descriptors)
        {
            return descriptors.First();
        }

        private static bool IsIgnored(ControllerActionDescriptor descriptor)
        {
            return descriptor.MethodInfo.IsDefined(typeof(RouteIgnoreAttribute), false);
        }
    }
}
