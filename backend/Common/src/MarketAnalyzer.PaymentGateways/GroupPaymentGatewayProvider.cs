using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.PaymentGateways
{
    internal class GroupPaymentGatewayProvider : IGroupPaymentGatewayProvider
    {
        private readonly IEnumerable<IGroupPaymentGateway> _gateways;

        public GroupPaymentGatewayProvider(IEnumerable<IGroupPaymentGateway> gateways)
        {
            _gateways = gateways;
        }

        public IGroupPaymentGateway Resolve(string uniqueName)
        {
            var gateway = _gateways.FirstOrDefault(e => e.UniqueName == uniqueName);
            return gateway ?? throw new NotSupportedException($"The group payment gateway with unique name '{uniqueName}' was not found.");
        }
    }
}
