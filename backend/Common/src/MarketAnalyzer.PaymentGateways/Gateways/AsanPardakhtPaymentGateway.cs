using Microsoft.Net.Http.Headers;
using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.PaymentGateways
{
    internal class AsanPardakhtPaymentGateway : PaymentGatewayBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly Dictionary<int, string> _tokenStatusMapping = new()
        {
            //request succeeded. Find the token in the form of string in body of the response
            { 200, "عملیات با موفقیت انجام شد. کاربر را به درگاه پرداخت هدایت کنید." },

            { 400, "bad request" },
            { 401, "unauthorized. probably wrong or unsent header(s)" },
            { 473, "invalid merchant username or password" },
            { 474, "invalid incoming request machine ip. check response body to see your actual public IP address" },
            { 475, "invoice identifier is not a number" },
            { 476, "request amount is not a number" },
            { 477, "request local date length is invalid" },
            { 478, "request local date is not in valid format" },
            { 479, "invalid service type id" },
            { 480, "invalid payer id" },
            { 481, "incorrect settlement description format" },
            { 482, "settlement slices does not match total amount" },
            { 483, "unregistered iban" },
            { 484, "internal error for other reasons" },
            { 485, "invalid local date" },
            { 486, "amount not in range" },
            { 487, "service not found or not available for merchant" },
            { 488, "invalid default callback" },
            { 489, "duplicate local invoice id" },
            { 490, "merchant disabled or misconfigured" },
            { 491, "too many settlement destinations" },
            { 492, "unprocessable request" },
            { 493, "error processing special request for other reasons like business restrictions" },
            { 494, "invalid payment_id for governmental payment" },
            { 495, "invalid referenceId in additionalData" },
            { 496, "invalid json in additionalData" },
            { 497, "invalid payment_id location" },
            { 571, "misconfiguration" },
            { 572, "misconfiguration" },
            { 573, "misconfiguraed valid ips for configuration" },
            { 574, "internal error in uthorization" },
            { 575, "no valid ibans found for merchant" },
            { 576, "internal error" },
            { 577, "internal error" },
            { 578, "no default sharing is defined for merchant" },
            { 579, "cant submit ibans with default sharing endpoint" },
            { 580, "error processing special request" },
        };
        private readonly Dictionary<int, string> _tranResultStatusMapping = new()
        {
            { 200, "transaction data found and operation succeeded" },
            { 400, "bad request" },
            { 401, "unauthorized. probably wrong or unsent header(s)" },
            { 471, "identity not trusted to proceed" },
            { 472, "no records found" },
            { 571, "error in processing" }
        };
        private readonly Dictionary<int, string> _verifyStatusMapping = new()
        {
            { 200, "تراكنش با موفقيت انجام شد" },
            { 400, "bad request" },
            { 401, "unauthorized. probably wrong or unsent header(s)" },
            { 471, "original transaction was failed" },
            { 472, "transaction already requested for verification" },
            { 473, "transaction already requested for reconcilation" },
            { 474, "transaction already requested for reversal" },
            { 475, "transaction already listed for reconcilation" },
            { 476, "transaction already listed for reversal" },
            { 477, "identity not trusted to proceed" },
            { 478, "verification already cancelled" },
            { 571, "not yet processed" },
            { 572, "transaction status undetermined" },
            { 573, "unable to request for verification due to an internal error" }
        };

        public AsanPardakhtPaymentGateway(IHttpClientFactory clientFactory) : base("AsanPardakht")
        {
            _clientFactory = clientFactory;
        }

        protected override Dictionary<int, string> StatusDescriptions => throw new NotSupportedException();

        public override async Task<PaymentRequestResult> RequestAsync(OnlinePaymentRequest request, CancellationToken cancellation = default)
        {
            var config = Deserialize<GatewayConfig>(request.GatewayConfig);
            using var client = CreateClient(config);
            var response = await client.PostAsJsonAsync("v1/Token", new TokenRequest()
            {
                ServiceTypeId = 1,
                MerchantConfigurationId = config.MerchantId,
                AmountInRials = (int)request.Amount,
                LocalInvoiceId = request.Id,
                CallbackURL = request.CallbackUrl,
                LocalDate = DateTime.Now.ToString("yyyyMMdd HHmmss", CultureInfo.InvariantCulture),
                PaymentId = "0",
                UseDefaultSharing = false,
                AdditionalData = null,
                SettlementPortions = null
            }, cancellation);
            var statusCode = (int)response.StatusCode;
            if (statusCode == 200)
            {
                var token = await response.Content.ReadAsStringAsync(cancellation);
                return new PaymentRequestResult()
                {
                    Succeeded = true,
                    ResponseCode = statusCode,
                    RequestId = token,
                    Error = null
                };
            }
            else
            {
                return new PaymentRequestResult()
                {
                    Succeeded = false,
                    ResponseCode = statusCode,
                    RequestId = null,
                    Error = _tokenStatusMapping.ContainsKey(statusCode) ?
                        _tokenStatusMapping[statusCode] :
                        $"خطای شماره ({statusCode}): با پشتیبانی تماس بگیرید."
                };
            }
        }

        public override async Task<TransactionResult> ProcessTransactionAsync(ProcessTransactionRequest request, CancellationToken cancellation = default)
        {
            string description;
            var succeeded = false;
            TransactionData transctionData = null;
            var config = Deserialize<GatewayConfig>(request.GatewayConfig);
            using var client = CreateClient(config);
            var url = $"/v1/TranResult?LocalInvoiceId={request.InternalRequestId}&MerchantConfigurationId={config.MerchantId}";
            var response = await client.GetAsync(url, cancellation);
            var statusCode = (int)response.StatusCode;
            if (statusCode == 200)
            {
                transctionData = await response.Content.ReadFromJsonAsync<TransactionData>(cancellationToken: cancellation);
                if (transctionData != null)
                {
                    if (int.Parse(transctionData.Amount) != (int)request.Amount)
                    {
                        description = "مبلغ تراکنش مغایرت دارد.";
                    }
                    else if (transctionData.RefID != request.RequestId)
                    {
                        description = "شناسه درخواست تراکنش مغایرت دارد.";
                    }
                    else
                    {
                        succeeded = true;
                        description = "تراکنش موفقیت آمیز بوده است.";
                    }
                }
                else
                {
                    description = "خطایی در پردازش نتیجه تراکنش رخ داده است.";
                }
            }
            else if (!_tranResultStatusMapping.TryGetValue(statusCode, out description))
            {
                description = $"خطای شماره ({statusCode}): با پشتیبانی تماس بگیرید.";
            }

            return new TransactionResult()
            {
                Succeeded = succeeded,
                StatusCode = statusCode,
                Description = description,
                TransactionId = transctionData.PayGateTranID,
                TransctionData = transctionData == null ? null : JsonSerializer.Serialize(transctionData)
            };
        }

        public override async Task<VerifyTransactionResult> VerifyTransactionAsync(VerifyTransactionRequest request, CancellationToken cancellation = default)
        {
            var config = Deserialize<GatewayConfig>(request.GatewayConfig);
            using var client = CreateClient(config);
            var response = await client.PostAsJsonAsync("v1​/Verify", new
            {
                merchantConfigurationId = config.MerchantId,
                payGateTranId = long.Parse(request.TransactionId)
            }, cancellationToken: cancellation);

            var statusCode = (int)response.StatusCode;
            return new VerifyTransactionResult()
            {
                Succeeded = statusCode == 200,
                StatusCode = statusCode,
                Description = _verifyStatusMapping.ContainsKey(statusCode) ?
                    _verifyStatusMapping[statusCode] :
                    $"خطای شماره ({statusCode}): با پشتیبانی تماس بگیرید."
            };
        }

        public override string GetCodeDescription(int code)
        {
            if (_verifyStatusMapping.TryGetValue(code, out string message))
            {
                return message;
            }

            if (_tranResultStatusMapping.TryGetValue(code, out message))
            {
                return message;
            }

            if (_tokenStatusMapping.TryGetValue(code, out message))
            {
                return message;
            }

            return base.GetCodeDescription(code);
        }

        protected override string FormatResponseContent(string content, ProcessRequestOptions request)
        {
            return content
                .Replace("[TargetUrl]", request.GatewayUrl)
                .Replace("[Token]", request.RequestId);
        }

        private HttpClient CreateClient(GatewayConfig config)
        {
            var client = _clientFactory.CreateClient("AsanPardakhtHttpClient");
            client.BaseAddress = new Uri("https://ipgrest.asanpardakht.ir/");
            client.DefaultRequestHeaders.Add(HeaderNames.ContentType, "application/json");
            client.DefaultRequestHeaders.Add("usr", config.MerchantUsername);
            client.DefaultRequestHeaders.Add("pwd", config.MerchantPassword);
            return client;
        }

        private class GatewayConfig
        {
            public int MerchantId { get; set; }
            public string MerchantUsername { get; set; }
            public string MerchantPassword { get; set; }
        }

        private class TokenRequest
        {
            /// <summary>
            /// کد پیکربندی پذیرنده
            /// </summary>
            public int MerchantConfigurationId { get; set; }
            /// <summary>
            /// کد نوع سرویس. برای انجام خرید کد 1 را ارسال بفرمائید
            /// </summary>
            public int ServiceTypeId { get; set; }
            /// <summary>
            /// شناسه فاکتور. 
            /// لازم است پذیرنده در هنگام ارسال درخواست خرید خود، شناسه منحصر به فرد و عددی را ارسال نماید. 
            /// این شناسه می تواند هر عدد مثبت صحیحی باشد.
            /// این شناسه نمی تواند به هیچ وجه تکراری باشد.
            /// </summary>
            public long LocalInvoiceId { get; set; }
            /// <summary>
            /// مبلغ فاکتور به ریال
            /// </summary>
            public int AmountInRials { get; set; }
            /// <summary>
            /// تاریخ میلادی و زمان ارسال درخواست.  YYYYMMDD HHMMSS
            /// </summary>
            public string LocalDate { get; set; }
            /// <summary>
            /// اطلاعات اضافی تراکنش را در این فیلد ارسال فرمائید.
            /// </summary>
            public string AdditionalData { get; set; }
            /// <summary>
            /// آدرس دریافت نتیجه درخواست از درگاه پرداخت
            /// </summary>
            public string CallbackURL { get; set; }
            /// <summary>
            /// شناسه پرداخت. چنانچه عملیات شما از این ویژگی پشتیبانی نمی کند فیلد مزبور را 0 ارسال کنید.
            /// </summary>
            public string PaymentId { get; set; }
            public bool UseDefaultSharing { get; set; }
            /// <summary>
            /// درخواست تقسیم وجوه به حساب‌های مختلف
            /// </summary>
            public ICollection<Settlementportion> SettlementPortions { get; set; }
        }

        private class Settlementportion
        {
            public string Iban { get; set; }
            public int AmountInRials { get; set; }
            public string PaymentId { get; set; }
        }

        private class TransactionData
        {
            /// <summary>
            /// شماره کارت ماسک شده
            /// </summary>
            public string CardNumber { get; set; }

            /// <summary>
            /// شماره پیگیری شبکه بانکی
            /// </summary>
            public string Rrn { get; set; }

            /// <summary>
            /// توکنی که در هنگام ارجاع مشتری از سایت پذیرنده به صفحه پرداخت، از سوی پذیرنده به صفحه درگاه ارسال شده است.
            /// </summary>
            public string RefID { get; set; }

            /// <summary>
            /// مبلغ تراکنش انجام شده
            /// </summary>
            public string Amount { get; set; }

            /// <summary>
            /// شماره تراکنش سوئیچ. این مقدار را برای عملیات تکمیلی نیاز دارید
            /// </summary>
            public string PayGateTranID { get; set; }

            /// <summary>
            /// شماره فاکتور محلی اعلام شده از سوی پذیرنده
            /// </summary>
            public string SalesOrderID { get; set; }

            /// <summary>
            /// کد نوع سروی س ارسال شده به متد Token
            /// </summary>
            public int ServiceTypeId { get; set; }

            #region [Special]

            /// <summary>
            /// نتیجه انجام تراکنش در سرویس مربوطه عدد 500 یا 600 به معنی تراکنش موفق می باشد 
            /// عدد 700 به معنی ناموفق است و پول به حساب مشتری باز می گردد غیر از 3 عدد بالا، هر چه بود، خطا می باشد و نتیجه تراکنش نامشخص می گردد
            /// </summary>
            public string ServiceStatusCode { get; set; }
            public string DestinationMobile { get; set; }
            public int ProductId { get; set; }
            public string ProductNameFa { get; set; }
            public int ProductPrice { get; set; }
            public int OperatorId { get; set; }
            public string OperatorNameFa { get; set; }
            public int SimTypeId { get; set; }
            public string SimTypeTitleFa { get; set; }
            public string BillId { get; set; }
            public string PayId { get; set; }
            public string BillOrganizationNameFa { get; set; }
            public DateTime PayGateTranDate { get; set; }
            public int PayGateTranDateEpoch { get; set; }
            public string Hash { get; set; }

            #endregion
        }
    }
}
