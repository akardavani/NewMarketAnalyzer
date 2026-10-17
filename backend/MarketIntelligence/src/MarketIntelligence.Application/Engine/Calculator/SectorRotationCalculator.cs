using MarketIntelligence.Domain;

namespace MarketIntelligence.Application
{
    public static class SectorRotationCalculator
    {
        public static decimal Score(IReadOnlyList<MarketCandle> industry)
        {
            var r30 =
                ReturnCalculator.CalculateReturn(
                    industry,
                    30);

            var r60 =
                ReturnCalculator.CalculateReturn(
                    industry,
                    60);

            var r90 =
                ReturnCalculator.CalculateReturn(
                    industry,
                    90);

            return
                r30 * 0.5m +
                r60 * 0.3m +
                r90 * 0.2m;
        }
    }









}
