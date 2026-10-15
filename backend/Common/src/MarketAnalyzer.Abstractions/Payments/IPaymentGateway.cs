using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface IPaymentGateway
    {
        /// <summary>
        /// نام یکتای درگاه پرداخت
        /// </summary>
        string UniqueName { get; }
        /// <summary>
        /// شرح کد وضعیت
        /// </summary>
        /// <param name="code">کد وضعیت</param>
        string GetCodeDescription(int code);
        /// <summary>
        /// درخواست پرداخت
        /// </summary>
        Task<PaymentRequestResult> RequestAsync(OnlinePaymentRequest request, CancellationToken cancellation = default);
        /// <summary>
        /// پردازش درخواست پرداخت
        /// </summary>
        Task<IActionResult> ProcessRequestAsync(ProcessRequestOptions request, CancellationToken cancellation = default);
        /// <summary>
        /// تولید محتوای HTML برای هدایت کاربر به صفحه پرداخت
        /// </summary>
        Task<string> GenerateHtmlContentAsync(ProcessRequestOptions request, CancellationToken cancellation = default);
        /// <summary>
        /// پردازش تراکنش
        /// </summary>
        Task<TransactionResult> ProcessTransactionAsync(ProcessTransactionRequest request, CancellationToken cancellation = default);
        /// <summary>
        /// اعتبارسنجی تراکنش
        /// </summary>
        Task<VerifyTransactionResult> VerifyTransactionAsync(VerifyTransactionRequest request, CancellationToken cancellation = default);
    }
}
