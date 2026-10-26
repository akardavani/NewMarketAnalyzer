using MarketAnalyzer.MarketIntelligence.Domain;

namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>رویداد شرکتی آن‌طور که ICorporateActionProvider گزارش می‌کند.</summary>
    public sealed class CorporateActionInfo
    {
        /// <summary>کلید استاندارد نماد.</summary>
        public required string SymbolKey { get; init; }

        /// <summary>نوع رویداد.</summary>
        public required CorporateActionType ActionType { get; init; }

        /// <summary>تاریخ اجرای رویداد.</summary>
        public required DateOnly EffectiveDate { get; init; }

        /// <summary>مقدار خام رویداد (مثلاً درصد افزایش سرمایه یا سود هر سهم).</summary>
        public decimal? Amount { get; init; }

        /// <summary>ضریب تعدیل اگر منبع آن را مستقیم بدهد؛ در غیر این صورت IAdjustmentFactorCalculator آن را حساب می‌کند.</summary>
        public decimal? AdjustmentFactor { get; init; }

        /// <summary>توضیح اختیاری.</summary>
        public string? Description { get; init; }
    }
}
