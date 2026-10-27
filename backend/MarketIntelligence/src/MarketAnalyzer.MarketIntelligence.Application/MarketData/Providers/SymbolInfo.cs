namespace MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers
{
    /// <summary>هویت یک نماد آن‌طور که Provider گزارش می‌کند؛ پیش از ذخیره به MarketSymbol نگاشت می‌شود.</summary>
    public sealed class SymbolInfo
    {
        /// <summary>کلید استاندارد نماد در بازار؛ Provider قاعدهٔ استانداردسازی بازار را اعمال کرده است.</summary>
        public required string SymbolKey { get; init; }

        /// <summary>شناسهٔ نماد در منبع بیرونی (مثلاً InsCode)؛ ممکن است خالی باشد.</summary>
        public string? ExternalId { get; init; }

        /// <summary>واحد پول قیمت این نماد اگر با واحد پیش‌فرض بازار (در ProviderDescriptor) فرق دارد؛ خالی یعنی همان واحد بازار.</summary>
        public string? CurrencyCode { get; init; }

        /// <summary>نام انگلیسی یا لاتین.</summary>
        public string? Name { get; init; }

        /// <summary>نام فارسی.</summary>
        public string? PersianName { get; init; }

        /// <summary>آیا منبع نماد را فعال می‌داند؟</summary>
        public bool IsActive { get; init; } = true;

        /// <summary>اطلاعات اضافهٔ مختص بازار به‌صورت JSON.</summary>
        public string? MetadataJson { get; init; }

        /// <summary>نسخهٔ ساختار MetadataJson.</summary>
        public int MetadataVersion { get; init; }
    }
}
