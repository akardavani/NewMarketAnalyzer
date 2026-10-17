using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.PaymentGateways
{
    internal class PaymentGatewayProvider : IPaymentGatewayProvider
    {
        private readonly IEnumerable<IPaymentGateway> _gateways;

        public PaymentGatewayProvider(IEnumerable<IPaymentGateway> gateways)
        {
            _gateways = gateways;
        }

        public IPaymentGateway Resolve(string uniqueName)
        {
            var gateway = _gateways.FirstOrDefault(e => e.UniqueName == uniqueName);
            return gateway ?? throw new NotSupportedException($"The payment gateway with unique name '{uniqueName}' was not found.");
        }
    }
}
