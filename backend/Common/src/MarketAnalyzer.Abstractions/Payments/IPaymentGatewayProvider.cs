namespace MarketAnalyzer.Abstractions
{
    public interface IPaymentGatewayProvider
    {
        IPaymentGateway Resolve(string uniqueName);
    }
}
