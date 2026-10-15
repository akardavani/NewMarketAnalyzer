namespace MarketAnalyzer.Abstractions
{
    public interface IRequest<out TResponse>
    {
    }

    public interface IRequest : IRequest<Void>
    {
    }
}