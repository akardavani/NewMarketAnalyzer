namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>یک قرارداد آتی بورس کالا از فهرست ثابت (Resources/ime.symbols.json).</summary>
    internal sealed class ImeContract
    {
        /// <summary>کد قرارداد (مثل SILDY01).</summary>
        public string Key { get; set; } = "";

        /// <summary>نام فارسی (با ی و ک فارسی یکدست شده).</summary>
        public string PersianName { get; set; } = "";

        /// <summary>سررسید به میلادی (yyyy-MM-dd).</summary>
        public string DeliveryDate { get; set; } = "";

        /// <summary>سررسید به شمسی.</summary>
        public string? PersianDeliveryDate { get; set; }

        /// <summary>دارایی پایه یا نماد مرتبط.</summary>
        public string? Parent { get; set; }
    }
}
