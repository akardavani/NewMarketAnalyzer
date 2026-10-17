using System;

namespace MarketAnalyzer.Abstractions
{
    public class ClientExposedException : Exception
    {
        public ClientExposedException(string message) : base(message)
        {

        }
    }

    public class NavigationNotLoadedException : Exception
    {
        public NavigationNotLoadedException(string propertyName) : base($"The {propertyName} not loaded.")
        {

        }
    }
}
