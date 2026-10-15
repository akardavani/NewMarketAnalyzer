namespace MarketAnalyzer.Api.Models
{
    public enum AppState
    {
        /// <summary>
        /// نیازمند ایجاد تنظیمات
        /// </summary>
        RequiredSettings,
        /// <summary>
        /// نیازمتد ایجاد سال مالی
        /// </summary>
        RequiredFiscalYear,
        /// <summary>
        /// آماده به کار
        /// </summary>
        Ready
    }
}
