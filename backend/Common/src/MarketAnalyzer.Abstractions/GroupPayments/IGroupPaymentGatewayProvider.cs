namespace MarketAnalyzer.Abstractions
{
    public interface IGroupPaymentGatewayProvider
    {
        IGroupPaymentGateway Resolve(string uniqueName);
    }
}
