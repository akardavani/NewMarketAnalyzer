namespace MarketAnalyzer.SharedKernel
{
    public class FundTypeAccounts
    {
        /// <summary>
        /// حساب سرمایه گذاری در صندوق
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
        /// حساب سود(زیان) ناشی از ابطال صندوق
        /// </summary>
        public required AccountValue RedemptionRevenueAccount { get; init; }
        /// <summary>
        /// حساب ارزشیابی در صندوق
        /// </summary>
        public required AccountValue EvaluationAccount { get; init; }
        /// <summary>
        /// حساب درآمد(هزینه) تغییر ارزش سرمایه گذاری در صندوق
        /// </summary>
        public required AccountValue AssetValueChangesRevenueAccount { get; init; }
    }
}
