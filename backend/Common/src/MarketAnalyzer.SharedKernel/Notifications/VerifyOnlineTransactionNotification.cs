using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.SharedKernel
{
    /// <summary>
    /// 
    /// </summary>
    public class VerifyOnlineTransactionNotification : INotification
    {
        /// <summary>
        /// شناسه درخواست واریز
        /// </summary>
        public required long DepostId { get; init; }
        /// <summary>
        /// حساب معین بستانکار که مبلغ واریز شده به آن سند میخورد
        /// </summary>
        public required AccountValue CreditAccount { get; init; }
        /// <summary>
        /// شناسه اپراتور
        /// </summary>
        public required int OperatorId { get; init; }
    }
}

