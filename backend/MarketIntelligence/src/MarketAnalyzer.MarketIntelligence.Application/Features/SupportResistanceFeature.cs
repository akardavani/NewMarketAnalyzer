using MarketAnalyzer.MarketIntelligence.Infrastructure;

namespace MarketAnalyzer.MarketIntelligence.Application
{
    public sealed class SupportResistanceFeature : IFeature
    {
        public string Name => FeaturesName.SupportResistancePosition;

        public decimal Compute(StockAnalysisInput input, FeatureSet features)
        {
            var distSupport = input.Metrics.DistanceToSupport;
            var distResistance = input.Metrics.DistanceToResistance;

            // اگر هیچکدام نبود → موقعیت خنثی وسط کانال
            if (distSupport is null && distResistance is null)
                return 0.5m;

            // اگر فقط مقاومت نبود → نزدیک حمایت هستیم (به سمت صفر)
            if (distResistance is null)
                return 0.2m;

            // اگر فقط حمایت نبود → نزدیک مقاومت هستیم (به سمت یک)
            if (distSupport is null)
                return 0.8m;

            // هر دو مقدار وجود دارند → نرمال‌سازی
            var s = distSupport.Value;
            var r = distResistance.Value;

            if (s + r == 0)
                return 0.5m;

            var pos = s / (s + r);

            return Math.Clamp(pos, 0m, 1m);
        }
    }

}
