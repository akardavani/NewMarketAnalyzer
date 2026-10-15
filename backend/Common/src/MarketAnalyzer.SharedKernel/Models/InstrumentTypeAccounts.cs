namespace MarketAnalyzer.SharedKernel
{
    /// <summary>
    /// حسابهای معین متناظر برای نوع نماد
    /// </summary>
    public class InstrumentTypeAccounts
    {
        /// <summary>
        /// حساب سرمایه گذاری در سهام
        /// </summary>
        public required AccountValue InvestingAccount { get; init; }
        /// <summary>
        /// حساب مالیات
        /// </summary>
        public required AccountValue TaxAccount { get; init; }
        /// <summary>
        /// حساب کارمزد
        /// </summary>
        public required AccountValue CommissionAccount { get; init; }
        /// <summary>
        /// حساب سود(زیان) ناشی از فروش سهام - باید جایگزین شود
        /// </summary>
        public required AccountValue SalesRevenueAccount { get; init; }
        /// <summary>
        /// حساب ارزشیابی در سهام
        /// </summary>
        public required AccountValue EvaluationAccount { get; init; }
        /// <summary>
        /// حساب درآمد(هزینه) تغییر ارزش سرمایه گذاری در سهام
        /// </summary>
        public required AccountValue AssetValueChangesIncomeAccountBalance { get; init; }
    }
}
