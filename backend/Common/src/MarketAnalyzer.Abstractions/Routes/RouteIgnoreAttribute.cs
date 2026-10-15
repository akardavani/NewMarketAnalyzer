using System;

namespace MarketAnalyzer.Abstractions.Routes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RouteIgnoreAttribute : Attribute
    {
    }
}
