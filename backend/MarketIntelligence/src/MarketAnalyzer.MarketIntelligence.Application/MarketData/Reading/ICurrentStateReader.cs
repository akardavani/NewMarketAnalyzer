namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Reading
{
    /// <summary>خواندن آخرین وضعیت نمادها.</summary>
    public interface ICurrentStateReader
    {
        /// <summary>آخرین وضعیت نمادهای داده‌شده را می‌خواند؛ نمادهای بدون وضعیت در نتیجه نمی‌آیند.</summary>
        Task<IReadOnlyList<CurrentStateDto>> GetStatesAsync(IReadOnlyCollection<int> symbolIds, CancellationToken cancellationToken);
    }
}
