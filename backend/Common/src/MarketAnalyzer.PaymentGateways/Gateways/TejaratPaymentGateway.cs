using Microsoft.AspNetCore.Http;
using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.PaymentGateways
{
    internal class TejaratPaymentGateway : PaymentGatewayBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly Dictionary<int, string> _statusDescriptions = new()
        {
            { 0, "تراكنش با موفقيت انجام شد" },
            { 5, "از انجام تراکنش صرف نظر شد" },
            { 3, "پذیرنده فروشگاهی نا معتبر است" },
            { 64, "مبلغ تراکنش نادرست است،جمع مبالغ تقسیم وجوه برابر مبلغ کل تراکنش نمی باشد" },
            { 94, "تراکنش تکراری است" },
            { 25, "تراکنش اصلی یافت نشد" },
            { 77, "روز مالی تراکنش نا معتبر است" },
            { 63, "کد اعتبار سنجی پیام نا معتبر است" },
            { 97, "کد تولید کد اعتبار سنجی نا معتبر است" },
            { 30, "فرمت پیام نادرست است" },
            { 86, "شتاب در حال Sign Off است" },
            { 55, "رمز کارت نادرست است" },
            { 40, "عمل درخواستی پشتیبانی نمی شود" },
            { 57, "انجام تراکنش مورد درخواست توسط پایانه انجام دهنده مجاز نمی باشد" },
            { 58, "انجام تراکنش مورد درخواست توسط پایانه انجام دهنده مجاز نمی باشد" },
            //{ 63, "تمهیدات امنیتی نقض گردیده است" },
            { 96, "قوانین سامانه نقض گردیده است ، خطای داخلی سامانه" },

            { 2, "تراکنش قبلا برگشت شده است" },
            { 54, "تاریخ انقضا کارت سررسید شده است" },
            { 62, "کارت محدود شده است" },
            { 75, "تعداد دفعات ورود رمز اشتباه از حد مجاز فراتر رفته است" },
            { 14, "اطلاعات کارت صحیح نمی باشد" },
            { 17, "كاربر از انجام تراكنش منصرف شده است" },
            { 51, "موجودی حساب کافی نمی باشد" },
            { 56, "اطلاعات کارت یافت نشد" },
            { 61, "مبلغ تراکنش بیش از حد مجاز است" },
            { 65, "تعداد دفعات انجام تراکنش بیش از حد مجاز است" },
            { 78, "کارت فعال نیست" },
            { 79, "حساب متصل به کارت بسته یا دارای اشکال است" },
            { 42, "کارت یا حساب مبدا در وضعیت پذیرش نمی باشد" },
            { 31, "عدم تطابق کد ملی خریدار با دارنده کارت" },
            { 98, "سقف استفاده از رمز دوم ایستا به پایان رسیده است" },

            { 901, "درخواست نا معتبر است" },
            { 902, "پارامترهای اضافی درخواست نامعتبر می باشد" },
            { 903, "شناسه پرداخت نامعتبر می باشد" },
            { 904, "اطلاعات مرتبط با قبض نا معتبر می باشد" },
            { 905, "شناسه درخواست نامعتبر می باشد" },
            { 906, "درخواست تاریخ گذشته است" },
            { 907, "آدرس بازگشت نتیجه پرداخت نامعتبر می باشد" },
            { 909, "پذیرنده نامعتبر می باشد" },
            { 910, "پارامترهای مورد انتظار پرداخت تسهیمی تامین نگردیده است" },
            { 911, "پارامترهای مورد انتظار پرداخت تسهیمی نا معتبر یا دارای اشکال می باشد" },
            { 912, "تراکنش درخواستی برای پذیرنده فعال نیست" },
            { 913, "تراکنش تسهیم برای پذیرنده فعال نیست" },
            { 914, "آدرس آی پی دریافتی درخواست نا معتبر می باشد" },
            { 915, "شماره پایانه نامعتبر می باشد" },
            { 916, "شماره پذیرنده نا معتبر می باشد" },
            { 917, "نوع تراکنش اعلام شده در خواست نا معتبر می باشد" },
            { 918, "پذیرنده فعال نیست" },
            { 919, "مبالغ تسهیمی ارائه شده با توجه به قوانین حاکم بر وضعیت تسهیم پذیرنده، نا معتبر است" },
            { 920, "شناسه نشانه نامعتبر می باشد" },
            { 921, "شناسه نشانه نامعتبر و یا منقضی شده است" },
            { 922, "نقض امنیت درخواست" },
            { 923, "ارسال شناسه پرداخت در تراکنش قبض مجاز نیست" },
            { 928, "مبلغ مبادله شده نا معتبر می باشد" },
            { 929, "شناسه پرداخت ارائه شده با توجه به الگوریتم متناظر نا معتبر می باشد" },
            { 930, "کد ملی ارائه شده نا معتبر می باشد" },
        };

        public TejaratPaymentGateway(IHttpClientFactory clientFactory) : base("Tejarat")
        {
            _clientFactory = clientFactory;
        }

        protected override Dictionary<int, string> StatusDescriptions => _statusDescriptions;

        public override async Task<PaymentRequestResult> RequestAsync(OnlinePaymentRequest request, CancellationToken cancellation = default)
        {
            var config = Deserialize<GatewayConfig>(request.GatewayConfig);
            var tokenRequest = new TokenRequest
            {
                TerminalId = config.TerminalId,
                AcceptorId = config.AcceptorId,
                RevertUri = request.CallbackUrl,
                Amount = (long)request.Amount,
                PaymentId = request.Id.ToString(),
                RequestId = request.Id.ToString(),
                TransactionType = TransactionTypes.Purchase,
                RequestTimestamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                AdditionalParameters = null,
                AsanShp = null,
                MultiplexParameters = null,
                BillInfo = null,
                CmsPreservationId = null
            };
            using var client = _clientFactory.CreateClient(Constants.TejaratHttpClientName);
            client.BaseAddress = new Uri("https://ikc.shaparak.ir/");
            var response = await client.PostAsJsonAsync("api/v3/tokenization/make", new
            {
                Request = tokenRequest,
                AuthenticationEnvelope = EnvelopeHenlper.Create(tokenRequest, config.PublicKey, config.PassPhrase, false)
            }, cancellation);
            response.EnsureSuccessStatusCode();
            var token = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellation);
            return new PaymentRequestResult
            {
                Succeeded = token.Status,
                RequestId = token.Result?.Token,
                ResponseCode = token.Status ? 0 : int.Parse(token.ResponseCode),
                Error = GetCodeDescription(int.Parse(token.ResponseCode))
            };
        }

        public override Task<TransactionResult> ProcessTransactionAsync(ProcessTransactionRequest request, CancellationToken cancellation = default)
        {
            TransactionResult result;
            var token = request.FormData["token"].ToString();
            if (request.RequestId != token)
            {
                result = new TransactionResult
                {
                    Succeeded = false,
                    StatusCode = -1,
                    TransactionId = null,
                    TransctionData = null,
                    Description = "کد امنیتی صحیح نمی باشد!"
                };
            }
            else
            {
                var transctionData = new TransctionData()
                {
                    Token = token,
                    ResponseCode = int.Parse(request.FormData["responseCode"]),
                    TerminalId = request.FormData["acceptorId"].ToString(),
                    RetrievalReferenceNumber = request.FormData["retrievalReferenceNumber"].ToString(),
                    SystemTraceAuditNumber = request.FormData["systemTraceAuditNumber"].ToString()
                };

                result = new TransactionResult
                {
                    StatusCode = transctionData.ResponseCode,
                    Succeeded = transctionData.ResponseCode == 0,
                    TransactionId = transctionData.RetrievalReferenceNumber,
                    TransctionData = JsonSerializer.Serialize(transctionData),
                    Description = GetCodeDescription(transctionData.ResponseCode)
                };
            }

            return Task.FromResult(result);
        }

        public override async Task<VerifyTransactionResult> VerifyTransactionAsync(VerifyTransactionRequest request, CancellationToken cancellation = default)
        {
            var config = Deserialize<GatewayConfig>(request.GatewayConfig);
            var transactionData = Deserialize<TransctionData>(request.TransactionData);

            using var client = _clientFactory.CreateClient(Constants.TejaratHttpClientName);
            client.BaseAddress = new Uri("https://ikc.shaparak.ir/");
            var response = await client.PostAsJsonAsync("api/v3/confirmation/purchase", new
            {
                config.TerminalId,
                TokenIdentity = request.RequestId,
                transactionData.SystemTraceAuditNumber,
                RetrievalReferenceNumber = request.TransactionId
            }, cancellationToken: cancellation);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<VerificationResponse>(cancellationToken: cancellation);
            return new VerifyTransactionResult()
            {
                Succeeded = result.Status,
                StatusCode = result.Status ? 0 : result.ResponseCode, //TODO: REVIEW
                Description = GetCodeDescription(result.ResponseCode) //TODO: REVIEW
            };
        }

        protected override string FormatResponseContent(string conetnt, ProcessRequestOptions request)
        {
            return conetnt
                .Replace("[TargetUrl]", request.GatewayUrl)
                .Replace("[Token]", request.RequestId);
        }

        #region [Private Types]

        private static class EnvelopeHenlper
        {
            public static AuthenticationEnvelope Create(TokenRequest request, string rsaPublicKey, string passPhrase, bool isMultiplex)
            {
                string baseString =
                request.TerminalId +
                  passPhrase +
                   request.Amount.ToString().PadLeft(12, '0') +
                  (isMultiplex ? "01" : "00") +
                  (isMultiplex ?
                   request.MultiplexParameters.Select(t => $"{t.Iban.Replace("IR", "2718")}{t.Amount.ToString().PadLeft(12, '0')}")
                  .Aggregate((a, b) => $"{a}{b}")
                  : string.Empty);

                using var myAes = Aes.Create();
                myAes.KeySize = 128;
                myAes.GenerateKey();
                myAes.GenerateIV();
                byte[] keyAes = myAes.Key;
                byte[] ivAes = myAes.IV;

                byte[] resultCoding = new byte[48];
                byte[] baseStringbyte = baseString.HexStringToByteArray();

                byte[] encrypted = EncryptStringToBytesAes(baseStringbyte, myAes.Key, myAes.IV);
                byte[] hsahash = SHA256.HashData(encrypted);

                resultCoding = CombinArray(keyAes, hsahash);

                return new AuthenticationEnvelope()
                {
                    Iv = ivAes.ByteArrayToHexString(),
                    Data = RsaData(resultCoding, rsaPublicKey).ByteArrayToHexString()
                };
            }

            private static byte[] EncryptStringToBytesAes(byte[] plainText, byte[] Key, byte[] IV)
            {
                using var aesAlg = Aes.Create();
                aesAlg.KeySize = 128;
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                return encryptor.TransformFinalBlock(plainText, 0, plainText.Length);
            }

            private static byte[] CombinArray(byte[] first, byte[] second)
            {
                byte[] bytes = new byte[first.Length + second.Length];
                Array.Copy(first, 0, bytes, 0, first.Length);
                Array.Copy(second, 0, bytes, first.Length, second.Length);
                return bytes;
            }

            private static byte[] RsaData(byte[] aesCodingResult, string publicKey)
            {
                publicKey = publicKey
                    .Replace("-----BEGIN PUBLIC KEY-----", "")
                    .Replace("-----END PUBLIC KEY-----", "");
                var csp = new RSACryptoServiceProvider();
                var data = Convert.FromBase64String(publicKey);
                csp.ImportSubjectPublicKeyInfo(data, out int _);
                return csp.Encrypt(aesCodingResult, false);
            }
        }

        private class GatewayConfig
        {
            /// <summary>
            /// شماره پایانه
            /// </summary>
            public string TerminalId { get; set; }
            /// <summary>
            /// شماره پذیرنده
            /// </summary>
            public string AcceptorId { get; set; }
            public string PublicKey { get; set; }
            public string PassPhrase { get; set; }
        }

        private class TokenRequest
        {
            /// <summary>
            /// مشخص کننده نوع تراکنش
            /// </summary>
            public string TransactionType { get; set; }

            /// <summary>
            /// شماره پایانه
            /// </summary>
            public string TerminalId { get; set; }

            /// <summary>
            /// شماره پذیرنده
            /// </summary>
            public string AcceptorId { get; set; }

            /// <summary>
            /// مبلغ تراکنش
            /// </summary>
            public long Amount { get; set; }

            /// <summary>
            /// آدرس ارسال پاسخ نتیجه عملیات
            /// </summary>
            public string RevertUri { get; set; }

            /// <summary>
            /// شامل اطلاعات مرتبط با پرداخت قبض
            /// </summary>
            public BillInfo BillInfo { get; set; }

            /// <summary>
            /// شامل اطلاعات مرتبط با خرید تسهیم
            /// </summary>
            public MultiplexParameter[] MultiplexParameters { get; set; }

            /// <summary>
            /// شناسه پرداخت
            /// </summary>
            public string PaymentId { get; set; }

            /// <summary>
            /// شناسه درخواست
            /// </summary>
            public string RequestId { get; set; }

            /// <summary>
            /// شناسه مرتبط با مشتری در سامانه های پذیرنده/پرداخت
            /// </summary>
            public string CmsPreservationId { get; set; }

            /// <summary>
            /// تاریخ و ساعت تولید درخواست در سامانه های پذیرنده
            /// </summary>
            public long RequestTimestamp { get; set; }

            /// <summary>
            /// پارامترهای اضافی
            /// </summary>
            public Dictionary<string, string> AdditionalParameters { get; set; }

            /// <summary>
            /// شامل اطلاعات مرتبط با خرید اعتباری
            /// </summary>
            public AsanShp AsanShp { get; set; }
        }

        private class BillInfo
        {
            /// <summary>
            /// شناسه قبض
            /// </summary>
            public string BillId { get; set; }

            /// <summary>
            /// شماره پرداخت قبض
            /// </summary>
            public string BillPaymentId { get; set; }
        }

        private class AsanShp
        {
            /// <summary>
            /// مبلغ پیش پرداخت در خرید اعتباری
            /// </summary>
            public long PrepaymentAmount { get; set; }

            /// <summary>
            /// مبلغ خرید اعتباری
            /// </summary>
            public long LoanAmount { get; set; }

            /// <summary>
            /// تعداد اقساط مرتبط با خرید اعتباری
            /// </summary>
            public int LoanCount { get; set; }
        }

        private class AuthenticationEnvelope
        {
            /// <summary>
            /// بردار تولیدی ، مورد استفاده در یک الگوریتم رمز نگاری متقارن
            /// </summary>
            public string Iv { get; set; }
            /// <summary>
            /// نتیجه عملیات رمز نگاری بر اسال مکانیزم پاکت دیجیتال
            /// </summary>
            public string Data { get; set; }
        }

        private class MultiplexParameter
        {
            /// <summary>
            /// شماره شبای واریزی
            /// </summary>
            public string Iban { get; set; }

            /// <summary>
            /// مبلغ پرداختی به ازای شماره شبای ارائه شده
            /// </summary>
            public long Amount { get; set; }
        }

        private class TransactionTypes
        {
            /// <summary>
            /// تراکنش خرید
            /// </summary>
            public const string Purchase = "Purchase";

            /// <summary>
            /// تراکنش پرداخت قبض
            /// </summary>
            public const string Bill = "Bill";

            /// <summary>
            /// خرید اعتباری به همراه پیش پرداخت
            /// </summary>
            public const string AsanShpWPP = "AsanShpWPP";
        }

        private class TokenResponse
        {
            /// <summary>
            /// نتیجه کلی عملیات
            /// </summary>
            public bool Status { get; set; }
            /// <summary>
            /// کد نتیجه عملیات
            /// </summary>
            public string ResponseCode { get; set; }
            /// <summary>
            /// توضیحات مرتبط با عملیات
            /// </summary>
            public string Description { get; set; }
            /// <summary>
            /// در صورتی که نتیجه کلی عملیات موفق باشد شامل اطلاعات مرتبط با نشانه خواهد بود
            /// </summary>
            public TokenData Result { get; set; }
        }

        private class TokenData
        {
            /// <summary>
            /// نشانه تخصیص داده شده
            /// </summary>
            public string Token { get; set; }
            /// <summary>
            /// زمان تولید نشانه
            /// </summary>
            public long InitiateTimeStamp { get; set; }
            /// <summary>
            /// زمان انقضاء نشانه
            /// </summary>
            public long ExpiryTimeStamp { get; set; }
            /// <summary>
            /// نوع تراکنش مرتبط با نشانه
            /// </summary>
            public string TransactionType { get; set; }
        }

        private class VerificationResponse
        {
            public bool Status { get; set; }
            public int ResponseCode { get; set; }
            public string Description { get; set; }
        }

        private class TransctionData
        {
            public string Token { get; set; }
            public int ResponseCode { get; set; }
            public string TerminalId { get; set; }
            public string RetrievalReferenceNumber { get; set; }
            public string SystemTraceAuditNumber { get; set; }
        }

        #endregion
    }
}
