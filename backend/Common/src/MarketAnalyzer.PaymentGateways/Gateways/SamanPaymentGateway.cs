using Microsoft.AspNetCore.Http;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.PaymentGateways.Saman;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using static MarketAnalyzer.PaymentGateways.Saman.PaymentIFBindingSoapClient;

namespace MarketAnalyzer.PaymentGateways
{
    internal class SamanPaymentGateway : PaymentGatewayBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly Dictionary<int, string> _statusDescriptions = new()
        {
            { 1, "کاربر انصراف داده است" },
            { 2, "پرداخت با موفقیت انجام شد" },
            { 3, "پرداخت انجام نشد." },
            { 4, "کاربر در بازه زمانی تعیین شده پاسخی ارسال نکرده است" },
            { 5, "پارامترهای ارسالی نامعتبر است." },
            { 8, "آدرس سرور پذیرنده نامعتبر است" },
            { 10, "توکن ارسال شده یافت نشد" },
            { 11, "با این شماره ترمینال فقط تراکنش های توکنی قابل پرداخت هستند." },
            { 12, "شماره ترمینال ارسال شده یافت نشد." },
            { -1, "خطای در پردازش اطلاعات ارسالی. )مشکل در یکی از ورودیها و ناموفق بودن فراخوانی متد برگشت تراکنش(" },
            { -3, "ورودیها حاوی کارکترهای غیرمجاز میباشند" },
            { -4, "کلمه عبور یا کد فروشنده اشتباه است" },
            { -6, "تراکنش قبلا برگشت داده شده است" },
            { -7, "رسید دیجیتالی تهی است." },
            { -8, "طول ورودیها بیشتر از حد مجاز است" },
            { -9, "وجود کارکترهای غیرمجاز در مبلغ برگشتی" },
            { -10, "رسید دیجیتالی به صورت Base64 نیست" },
            { -11, "طول ورودیها کمتر از حد مجاز است" },
            { -12, "مبلغ برگشتی منفی است" },
            { -13, "مبلغ برگشتی برای برگشت جزئی بیش از مبلغ برگشت نخوردهی رسید دیجیتالی است" },
            { -14, "چنین تراکنشی تعریف نشده است" },
            { -15, "مبلغ برگشتی به صورت اعشاری داده شده است" },
            { -16, "خطای داخلی سیستم" },
            { -17, "برگشت زدن جزیی تراکنش مجاز نمی باشد" },
            { -18, "IP Address فروشنده نا معتبر است" },
            //-------------------------------------------------------------------------------------------
            //Custom error
            { -99, "تایید نهایی از طریق برنامه‌ی کاربردی به دلیل خطای نامشخصی به درگاه بانک ارسال نشد" }
        };

        public SamanPaymentGateway(IHttpClientFactory clientFactory) : base("Saman")
        {
            _clientFactory = clientFactory;
        }

        public override byte[] EncryptionIV => Convert.FromBase64String("WHAyczV2OHkvQj9FKEcrSw==");
        public override byte[] EncryptionKey => Convert.FromBase64String("M3M2djl5JEImRSlIQE1jUQ==");
        protected override Dictionary<int, string> StatusDescriptions => _statusDescriptions;

        public override async Task<PaymentRequestResult> RequestAsync(OnlinePaymentRequest request, CancellationToken cancellation = default)
        {
            var config = Deserialize<GatewayConfig>(request.GatewayConfig);
            using var client = _clientFactory.CreateClient(Constants.SamanHttpClientName);
            var apiRequest = new
            {
                Action = "token",
                Amount = (long)request.Amount,
                //Wage = 0,
                //AffectiveAmount = 0,
                config.TerminalId,
                ResNum = request.Id.ToString(),
                RedirectURL = request.CallbackUrl,
                CellNumber = request.MobileNumber,
                ShaparakKycParams = new
                {
                    ThirdPartyCode = 16,
                    CardHolderNationalId = !string.IsNullOrWhiteSpace(request.NationalCode) ? Encrypt($"0|{request.NationalCode}|{new Random().Next(1, 9999999)}") : null
                }
            };
            var response = await client.PostAsJsonAsync("https://sep.shaparak.ir/MobilePG/MobilePayment", apiRequest, cancellation);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PaymentRequestResponse>(cancellationToken: cancellation);
            var succeeded = result.Status == 1;
            return new PaymentRequestResult()
            {
                Succeeded = succeeded,
                RequestId = result.Token,
                Error = result.ErrorDesc,
                ResponseCode = succeeded ? result.Status : int.Parse(result.ErrorCode)
            };
        }

        public override Task<TransactionResult> ProcessTransactionAsync(ProcessTransactionRequest request, CancellationToken cancellation = default)
        {
            TransactionResult result;
            var token = request.FormData["Token"];
            if (request.RequestId != token)
            {
                result = new TransactionResult()
                {
                    Succeeded = false,
                    StatusCode = -99999,
                    TransactionId = null,
                    TransctionData = null,
                    Description = "کد امنیتی صحیح نمی باشد!"
                };
            }
            else
            {
                var transactionData = new TransactionData()
                {
                    Amount = long.Parse(request.FormData["Amount"]),
                    MID = request.FormData["MID"],
                    RefNum = request.FormData["RefNum"],
                    ResNum = request.FormData["ResNum"],
                    RRN = request.FormData["RRN"],
                    SecurePan = request.FormData["SecurePan"],
                    State = request.FormData["State"],
                    Status = int.Parse(request.FormData["Status"]),
                    TerminalId = request.FormData["TerminalId"],
                    TraceNo = request.FormData["TraceNo"],
                    //Wage = long.Parse(httpContext.Request.Form["Wage"])
                };

                var succeeded = transactionData.Status == 2;
                result = new TransactionResult()
                {
                    Succeeded = succeeded,
                    StatusCode = transactionData.Status,
                    Description = GetCodeDescription(transactionData.Status),
                    TransactionId = succeeded ? transactionData.RefNum : null,
                    TransctionData = JsonSerializer.Serialize(transactionData)
                };
            }

            return Task.FromResult(result);
        }

        public override async Task<VerifyTransactionResult> VerifyTransactionAsync(VerifyTransactionRequest request, CancellationToken cancellation = default)
        {
            var config = Deserialize<GatewayConfig>(request.GatewayConfig);
            var service = new PaymentIFBindingSoapClient(EndpointConfiguration.PaymentIFBindingSoap12);
            var result = await service.verifyTransaction1Async(request.TransactionId, config.MerchantId);
            var succeeded = result > 0 && Math.Abs((decimal)result - request.Amount) < 2;
            var statusCode = (int)(succeeded ? result : (result < 0 ? result : -99));
            return new VerifyTransactionResult()
            {
                Succeeded = succeeded,
                StatusCode = statusCode,
                Description = GetCodeDescription(statusCode)
            };
        }

        protected override string FormatResponseContent(string conetnt, ProcessRequestOptions request)
        {
            return conetnt
                 .Replace("[TargetUrl]", request.GatewayUrl)
                 .Replace("[Token]", request.RequestId);
        }

      
        //private static string Encrypt(string plain)
        //{
        //    if (string.IsNullOrEmpty(plain))
        //    {
        //        return null;
        //    }

        //    using var aes = Aes.Create();
        //    using var encryptor = aes.CreateEncryptor(_encryptionKey, _encryptionIV);
        //    using var stream = new MemoryStream();
        //    using var cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
        //    stream.Write(Encoding.UTF8.GetBytes(plain));
        //    return Convert.ToBase64String(stream.ToArray());
        //}

        private class GatewayConfig
        {
            public string TerminalId { get; set; }
            public string MerchantId { get; set; }
        }

        private class PaymentRequestResponse
        {
            /// <summary>
            /// وضعیت درخواست 1 نشان از موفقیت و مقدار 1- عدم موفقیت را نشان میدهد.
            /// </summary>
            [JsonPropertyName("status")]
            public int Status { get; set; }
            [JsonPropertyName("errorCode")]
            public string ErrorCode { get; set; }
            [JsonPropertyName("errorDesc")]
            public string ErrorDesc { get; set; }
            [JsonPropertyName("token")]
            public string Token { get; set; }
        }

        private class TransactionData
        {
            public string MID { get; set; }
            public string State { get; set; }
            public int Status { get; set; }
            public string RRN { get; set; }
            public string RefNum { get; set; }
            public string ResNum { get; set; }
            public string TerminalId { get; set; }
            public string TraceNo { get; set; }
            public long Amount { get; set; }
            public long Wage { get; set; }
            public string SecurePan { get; set; }
        }
    }
}
