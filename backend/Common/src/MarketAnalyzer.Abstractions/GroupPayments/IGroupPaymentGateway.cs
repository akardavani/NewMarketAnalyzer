using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface IGroupPaymentGateway
    {
        /// <summary>
        /// نام یکتای درگاه پرداخت
        /// </summary>
        string UniqueName { get; }
        /// <summary>
        /// ایجاد تراکنش گروهی
        /// </summary>
        Task<BatchTransferResult> CreateBatchAsync(BatchTransferRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// استعلام وضعیت تراکنش گروهی
        /// </summary>
        Task<BatchStatusInquiryResult> InquiryBatchStatusAsync(BatchStatusInquiryRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// اعتبارسنجی شماره حسابهای مقصد
        /// </summary>
        Task<ValidateAccountsResult> ValidateAccountsAsync(ValidateAccountsRequest request, CancellationToken cancellation = default);
    }
}
