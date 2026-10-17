using MarketAnalyzer.Abstractions;
using MarketAnalyzer.PaymentGateways.Mellat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.PaymentGateways
{
    internal class MellatPaymentGateway : PaymentGatewayBase
    {
        private readonly PaymentGatewayClient _service;
        private readonly Dictionary<int, string> _statusDescriptions = new()
        {
            { 0, "تراكنش با موفقیت انجام شد" },
            { 11, "شماره كارت نامعتبر است" },
            { 12, "موجودی كافی نیست" },
            { 13, "رمز نادرست است" },
            { 14, "تعداد دفعات وارد كردن رمز بیش از حد مجاز است" },
            { 15, "كارت نامعتبر است" },
            { 16, "دفعات برداشت وجه بیش از حد مجاز است" },
            { 17, "كاربر از انجام تراكنش منصرف شده است" },
            { 18, "تاریخ انقضای كارت گذشته است" },
            { 19, "مبلغ برداشت وجه بیش از حد مجاز است" },
            { 111, "صادر كننده كارت نامعتبر است" },
            { 112, "خطای سوییچ صادر كننده كارت" },
            { 113, "پاسخی از صادر كننده كارت دریافت نشد" },
            { 114, "دارنده كارت مجاز به انجام این تراكنش نیست" },
            { 21, "پذیرنده نامعتبر است" },
            { 23, "خطای امنیتی رخ داده است" },
            { 24, "اطلاعات كاربری پذیرنده نامعتبر است" },
            { 25, "مبلغ نامعتبر است" },
            { 31, "پاسخ نامعتبر است" },
            { 32, "فرمت اطلاعات وارد شده صحیح نمی باشد" },
            { 33, "حساب نامعتبر است" },
            { 34, "خطای سیستمی" },
            { 35, "تاریخ نامعتبر است" },
            { 41, "شماره درخواست تكراری است" },
            { 42, "یافت نشد Sale تراكنش" },
            { 43, "داده شده است Verify قبلا درخواست" },
            { 44, "یافت نشد Verfiy درخواست" },
            { 45, "شده است Settle تراكنش" },
            { 46, "نشده است Settle تراكنش" },
            { 47, "یافت نشد Settle تراكنش" },
            { 48, "شده است Reverse تراكنش" },
            { 49, "یافت نشد Refund تراكنش" },
            { 412, "شناسه قبض نادرست است" },
            { 413, "شناسه پرداخت نادرست است" },
            { 414, "سازمان صادر كننده قبض نامعتبر است" },
            { 415, "زمان جلسه كاری به پایان رسیده است" },
            { 416, "خطا در ثبت اطلاعات" },
            { 417, "شناسه پرداخت كننده نامعتبر است" },
            { 418, "اشكال در تعریف اطلاعات مشتری" },
            { 419, "تعداد دفعات ورود اطلاعات از حد مجاز گذشته است" },
            { 421, "نامعتبر است IP" },
            { 51, "تراكنش تكراری است" },
            { 54, "تراكنش مرجع موجود نیست" },
            { 55, "تراكنش نامعتبر است" },
            { 61, "خطا در واریز" }
        };
        

        public MellatPaymentGateway() : base("Mellat")
        {
            _service = new PaymentGatewayClient();
        }

        public override byte[] EncryptionIV => Convert.FromBase64String("Qj9EKEcrS2JQZVNoVm1ZcQ==");
        public override byte[] EncryptionKey => Convert.FromBase64String("SCtNYlFlVGhXbVpxNHQ3dw==");
        protected override Dictionary<int, string> StatusDescriptions => _statusDescriptions;

        public override async Task<PaymentRequestResult> RequestAsync(OnlinePaymentRequest request, CancellationToken cancellation = default)
        {
            var config = Deserialize<GatewayConfig>(request.GatewayConfig);
            var response = await _service.bpPayRequestAsync(
                        terminalId: config.TerminalId,
                        userName: config.UserName,
                        userPassword: config.UserPassword,
                        orderId: request.Id,
                        amount: (long)request.Amount,
                        localDate: request.RequestDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                        localTime: request.RequestDate.ToString("HHmmss", CultureInfo.InvariantCulture),
                        additionalData: string.Empty,
                        callBackUrl: request.CallbackUrl,
                        payerId: config.PayerId,
                        mobileNo: request.MobileNumber,
                        encPan: Encrypt(request.CardNumber),
                        panHiddenMode: "1",
                        cartItem: request.DisplayName,
                        enc: EncryptNationalCode(request.NationalCode));
            var responseParts = response.Body.@return.Split(',');
            var responseCode = int.Parse(responseParts[0].Trim());
            var succeeded = responseCode == 0;
            return new PaymentRequestResult()
            {
                Succeeded = succeeded,
                ResponseCode = responseCode,
                RequestId = succeeded ? responseParts[1].Trim() : null,
                Error = succeeded ? null : GetCodeDescription(responseCode)
            };
        }

        protected override string FormatResponseContent(string content, ProcessRequestOptions request)
        {
            return content
                 .Replace("[TargetUrl]", request.GatewayUrl)
                 .Replace("[RefId]", request.RequestId);
        }

        public override Task<TransactionResult> ProcessTransactionAsync(ProcessTransactionRequest request, CancellationToken cancellation = default)
        {
            TransactionResult result;
            var refId = request.FormData["RefId"];
            if (request.RequestId != refId)
            {
                result = new TransactionResult
                {
                    Succeeded = false,
                    StatusCode = -1,
                    TransactionId = null,
                    TransctionData = null,
                    Description = "کد امنیتی صحیح نمی باشد!",
                };
            }
            else
            {
                var resultCode = int.Parse(request.FormData["ResCode"]);
                var transactionData = new TransactionData()
                {
                    RefId = refId,
                    ResCode = resultCode,
                    SaleOrderId = long.Parse(request.FormData["SaleOrderId"]),
                    SaleReferenceId = resultCode == 0 ? long.Parse(request.FormData["SaleReferenceId"]) : null
                };
                result = new TransactionResult()
                {
                    StatusCode = resultCode,
                    Succeeded = resultCode == 0,
                    Description = GetCodeDescription(resultCode),
                    TransctionData = JsonSerializer.Serialize(transactionData),
                    TransactionId = transactionData.SaleReferenceId?.ToString()
                };
            }

            return Task.FromResult(result);
        }

        public override async Task<VerifyTransactionResult> VerifyTransactionAsync(VerifyTransactionRequest request, CancellationToken cancellation = default)
        {
            int resultCode;

            var saleOrderId = request.InsternalRequestId;
            var saleReferenceId = long.Parse(request.TransactionId);
            var config = Deserialize<GatewayConfig>(request.GatewayConfig);

            try
            {
                var verifyResponse = await _service.bpVerifyRequestAsync(
                    terminalId: config.TerminalId,
                    userName: config.UserName,
                    userPassword: config.UserPassword,
                    orderId: saleOrderId,
                    saleOrderId: saleOrderId,
                    saleReferenceId: saleReferenceId);

                resultCode = int.Parse(verifyResponse.Body.@return);
            }
            catch (Exception)
            {
                var inquiryResponse = await _service.bpInquiryRequestAsync(
                    terminalId: config.TerminalId,
                    userName: config.UserName,
                    userPassword: config.UserPassword,
                    orderId: saleOrderId,
                    saleOrderId: saleOrderId,
                    saleReferenceId: saleReferenceId);

                resultCode = int.Parse(inquiryResponse.Body.@return);
            }

            //نیازی به این قسمت نیست، تنها جهت تسریع در واریز پول به حساب است
            if (resultCode == 0)
            {
                try
                {
                    await _service.bpSettleRequestAsync(
                        terminalId: config.TerminalId,
                        userName: config.UserName,
                        userPassword: config.UserPassword,
                        orderId: saleOrderId,
                        saleOrderId: saleOrderId,
                        saleReferenceId: saleReferenceId);
                }
                catch (Exception)
                {
                    //Logger.LogWarning(ex, "Settle payment failed for SaleReferenceId: {0}", saleReferenceId);
                    //LogException(ex);
                }
            }

            return new VerifyTransactionResult()
            {
                StatusCode = resultCode,
                Succeeded = resultCode == 0,
                Description = GetCodeDescription(resultCode)
            };
        }

        private class GatewayConfig
        {
            public long TerminalId { get; set; }
            public string UserName { get; set; }
            public string UserPassword { get; set; }
            public string PayerId { get; set; }
        }

        private class TransactionData
        {
            public string RefId { get; set; }
            public int ResCode { get; set; }
            public long SaleOrderId { get; set; }
            public long? SaleReferenceId { get; set; }
        }
    }
}