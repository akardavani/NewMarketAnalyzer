using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Cms.Domain
{
    /// <summary>
    /// تنظیمات پورتال
    /// </summary>
    public class PortalSetting : AggregateRoot<string>
    {
        /// <summary>
        /// محتوای صفحه راهنمای سرمایه گذاری
        /// </summary>
        public const string InvestmentGuidePageContent = nameof(InvestmentGuidePageContent);

        public PortalSetting(string id) : base(id)
        {
        }

        /// <summary>
        /// مقدار تنظیمات
        /// </summary>
        public string Value { get; private set; }
    }
}
