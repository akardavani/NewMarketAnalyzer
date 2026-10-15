using System;

namespace MarketAnalyzer.Infrastructure.Authentication.ApiKey
{
    /// <summary>
    /// Mark action model to is internal use and authorize by api key policy.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class InternalApiAttribute : Attribute
    {

    }
}
