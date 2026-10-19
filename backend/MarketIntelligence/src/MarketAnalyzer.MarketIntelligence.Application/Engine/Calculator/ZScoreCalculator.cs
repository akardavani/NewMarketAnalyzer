using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application;
internal class ZScoreCalculator
{
    /// <summary>
    /// پیدا کردن جامانده
    /// </summary>
    public static decimal ZScore(IReadOnlyList<decimal> values)
    {
        if (values.Count < 30)
            return 0;

        var current =
            values[^1];

        var avg =
            values.Average();

        var std =
            (decimal)Math.Sqrt(
                values
                .Average(x =>
                    Math.Pow(
                        (double)(x - avg),
                        2)));

        if (std == 0)
            return 0;

        return
            (current - avg)
            / std;
    }

    /// <summary>
    /// Z = -2 ==> جامانده
    /// Z = +2 ==> بیش از حد رشد کرده
    /// </summary>
    /// <param name="stock"></param>
    /// <param name="industry"></param>
    /// <returns></returns>
    public static decimal StockVsIndustryZScore(
        IReadOnlyList<MarketCandle> stock,
        IReadOnlyList<MarketCandle> industry)
    {
        var ratio =
            RatioSeries(
                stock,
                industry);

        return
            ZScore(ratio);
    }

    public static List<decimal> RatioSeries(
        IReadOnlyList<MarketCandle> stock,
        IReadOnlyList<MarketCandle> benchmark)
    {
        var count =
            Math.Min(
                stock.Count,
                benchmark.Count);

        var result =
            new List<decimal>();

        for (int i = 0; i < count; i++)
        {
            if (benchmark[i].Close <= 0)
                continue;

            result.Add(
                stock[i].Close
                /
                benchmark[i].Close);
        }

        return result;
    }

    public static decimal CalculateValueScore(decimal pe)
    {
        if (pe <= 0)
            return 0m;

        if (pe < 6) return 1m;
        if (pe < 10) return 0.8m;
        if (pe < 15) return 0.6m;
        if (pe < 20) return 0.4m;

        return 0.2m;
    }

    public static decimal EstimateFairValue(decimal eps,decimal pe)
    {
        if (eps <= 0)
            return 0;

        var targetPE = pe < 10 ? 12m : pe;

        return eps * targetPE;
    }
}

