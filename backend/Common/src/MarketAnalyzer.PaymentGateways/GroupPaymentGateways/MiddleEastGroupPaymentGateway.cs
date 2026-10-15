using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.PaymentGateways.GroupPaymentGateways
{
    internal partial class MiddleEastGroupPaymentGateway : IGroupPaymentGateway
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public const string DateFormat = "yyyy/MM/dd";
        public const string DateTimeFormat = "yyyy/MM/dd HH:mm:ss";
        private static readonly CultureInfo _culture = new("fa-IR");
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly bool _fake = true;
        private static readonly Dictionary<string, BatchTransferRequestEntry> _requests = [];

        public MiddleEastGroupPaymentGateway(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string UniqueName => "MiddleEast";

        public async Task<ValidateAccountsResult> ValidateAccountsAsync(ValidateAccountsRequest request, CancellationToken cancellation = default)
        {
            var config = GatewayConfig.Parse(request.Config);
            var parameters = new
            {
                items = request.Accounts.Select(e => e.AccountNumber).ToList()
            };

            var response = await PostAsync(config, parameters, "batchValidate", cancellation);
            if (IsErrorResponse(response))
            {
                var result = JsonSerializer.Deserialize<ValidateAccountsError>(response);
                return new ValidateAccountsResult()
                {
                    Succeeded = false,
                    Accounts = null,
                    ErrorCode = result.ErrorCode,
                    ErrorDescription = result.ErrorTitle
                };
            }
            else
            {
                var items = JsonSerializer
                    .Deserialize<List<BatchValidateResult>>(response)
                    .Select(e => new ValidateAccountsResult.Account()
                    {
                        Status = TranslateAccountStatus(e.AccountStatus),
                        InquiryAccountNumber = e.Destination,
                        AccountNumber = e.AccountNumber,
                        BankName = e.Bank,
                        HolderName = ExtractHolderName(e.Title),
                        Description = e.Title
                    })
                    .ToList();
                return new ValidateAccountsResult()
                {
                    Succeeded = true,
                    Accounts = items,
                    ErrorCode = null,
                    ErrorDescription = null
                };
            }
        }

        public async Task<BatchTransferResult> CreateBatchAsync(BatchTransferRequest request, CancellationToken cancellation = default)
        {
            //تعداد آیتمها نباید بیش از 30000 تا باشد
            if (request.Items.Count > 30_000)
            {
                throw new Exception($"Items count oveflow: Max {30_000}, Count: {request.Items.Count}");
            }

            //TODO: Is fake and to remove
            if (_fake)
            {
                var transactionId = DateTime.Now.Ticks.ToString();
                _requests[transactionId] = new BatchTransferRequestEntry()
                {
                    Request = request,
                    DateTime = DateTime.Now
                };
                return new BatchTransferResult()
                {
                    TransactionId = transactionId,
                    State = BatchTransferState.AwaitingOwnersApproval,
                    ErrorCode = null,
                    ErrorTitle = null,
                    Rejected = null
                };
            }

            var config = GatewayConfig.Parse(request.Config);
            var parameters = new
            {
                //شماره حساب مبدأ
                accountNo = request.AccountNumber,
                //شرح تراکنش
                description = request.Description,
                //بابت
                reason = request.Reason,
                //شماره مرجع
                customerRef = request.CustomerRef,
                //به منظور نمایش لیست رکوردهایی که به دلیل دریافت خطا پرداخت نمیشوند
                rejected = true,
                //لیست مقاصد فقط شماره شبا
                items = request.Items.Select(e => new
                {
                    //شماره شبای ذینفع
                    iban = e.AccountNumber,
                    //نام ذینفع
                    benName = e.HolderName,
                    //شرح تک تراکنش
                    benRemark = e.Description,
                    //مبلغ تک تراکنش
                    amount = e.Amount,
                    //شماره پرداخت
                    paymentId = e.PayerId,
                    //شماره مرجع - به بانک مرکزی ارسال نمیشود
                    referenceId = e.RequestId
                })
                .ToList()
            };
            var response = await PostAsync(config, parameters, "createBatchTransfer", cancellation);
            if (IsWaitingResponse(response))
            {
                var result = JsonSerializer.Deserialize<WaitingResponse>(response);
                return new BatchTransferResult()
                {
                    Rejected = null,
                    ErrorCode = null,
                    ErrorTitle = null,
                    TransactionId = result.Reference.ToString(),
                    State = BatchTransferState.AwaitingOwnersApproval
                };
            }
            else if (IsErrorResponse(response))
            {
                var result = JsonSerializer.Deserialize<ErrorResponse>(response);
                return new BatchTransferResult()
                {
                    Rejected = null,
                    TransactionId = null,
                    State = BatchTransferState.Failed,
                    ErrorCode = result.Error.Code,
                    ErrorTitle = result.Error.Message
                };
            }
            else
            {
                var result = JsonSerializer.Deserialize<BatchTransferResponse>(response);
                return new BatchTransferResult()
                {
                    State = result.ErrorCode == "0000" ? BatchTransferState.Entry : BatchTransferState.Failed,
                    TransactionId = result.TransactionId,
                    ErrorCode = result.ErrorCode,
                    ErrorTitle = result.ErrorDesc,
                    Rejected = result.RejectedList?
                        .Select(e => new BatchTransferResult.RejectedItem()
                        {
                            Amount = e.Amount,
                            AccountNumber = e.AccountNumber,
                            Error = e.Error,
                            PayerId = e.PaymentId,
                            RequestId = e.ReferenceId
                        })?.ToList()
                };
            }
        }

        public async Task<BatchStatusInquiryResult> InquiryBatchStatusAsync(BatchStatusInquiryRequest request, CancellationToken cancellation = default)
        {
            var config = GatewayConfig.Parse(request.Config);
            var parameters = new
            {
                //شماره مرجع تراکنش
                txnId = request.TransactionId
            };
            var response = await PostAsync(config, parameters, "batchTransferStatus", cancellation);
            if (IsErrorResponse(response))
            {
                var result = JsonSerializer.Deserialize<ValidateAccountsError>(response, _jsonSerializerOptions);
                return new BatchStatusInquiryResult()
                {
                    Status = BatchTransferState.Failed,
                    EnteredDate = default,
                    AuthDate = default,
                    TransactionId = request.TransactionId,
                    Commission = 0,
                    TotalAmount = 0,
                    Description = null,
                    ErrorCode = result.ErrorCode,
                    ErrorTitle = result.ErrorTitle,
                    Items = null
                };
            }
            else
            {
                var result = JsonSerializer.Deserialize<BatchTransferStatus>(response, _jsonSerializerOptions);
                var status = result.Status switch
                {
                    //زمانی که تراکنش توسط سرویس ثبت دستور پرداخت ثبت شده است Entry
                    "E" => BatchTransferState.Entry, //TODO
                    //در انتظار تائید
                    "U" => BatchTransferState.AwaitingOwnersApproval, //TODO
                    //تایید شده
                    "A" => BatchTransferState.Executed, //TODO
                    //برگشت خورده
                    "R" => BatchTransferState.RejectedByOwners, //TODO
                    //نامعین
                    _ => throw new Exception($"Invalid BatchTransferState: [{result.Status}]")
                };

                List<BatchStatusInquiryResult.Item> items = null;
                if (result.Status == "A")
                {
                    var details = await GetBatchTransferDetailsAsync(request, cancellation);
                    items = details.Items
                        .Select(e => new BatchStatusInquiryResult.Item()
                        {
                            Amount = e.Amount,
                            PayerId = e.PaymentId,
                            AccountNumber = e.ShebaNumber,
                            Description = e.RejectDescription,
                            ReferenceNumber = e.ReferenceNumber,
                            RejectDescription = e.RejectDescription,
                            Status = TranslateBatchTransferItemState(e.Status),
                            Date = DateTime.ParseExact(e.Date, DateFormat, _culture, DateTimeStyles.None),
                        })
                        .ToList();
                }

                return new BatchStatusInquiryResult()
                {
                    Status = status,
                    EnteredDate = DateTime.ParseExact(result.EnteredDate, DateTimeFormat, _culture, DateTimeStyles.None),
                    AuthDate = DateTime.ParseExact(result.AuthDate, DateTimeFormat, _culture, DateTimeStyles.None),
                    TransactionId = result.TransactionId,
                    Commission = result.PayaChargesAmount + result.SatnaChargesAmount,
                    TotalAmount = result.TotalAmount,
                    Description = result.Description,
                    ErrorCode = null,
                    ErrorTitle = null,
                    Items = items
                };
            }
        }

        private async Task<BatchTransferDetailsResponse> GetBatchTransferDetailsAsync(BatchStatusInquiryRequest request, CancellationToken cancellation = default)
        {
            var config = GatewayConfig.Parse(request.Config);
            var parameters = new
            {
                //شماره مرجع تراکنش
                txnId = request.TransactionId,
                //شماره مرجع ارسال شده در دستور پرداخت
                referenceId = null as string,
                //در صورت ارسال دو پارامتر به پاسخ اضافه میشوند: referenceId , rejectedIbanFrom
                rejected = true
            };
            var response = await PostAsync(config, parameters, "batchTransferDetails", cancellation);
            if (IsErrorResponse(response))
            {
                var result = JsonSerializer.Deserialize<ValidateAccountsError>(response, _jsonSerializerOptions);
                throw new Exception($"There was and error on call [BatchTransferDetails] service:\r\n[ErrorCode: {result.ErrorCode}, ErrorTitle: {result.ErrorTitle}]");
            }
            else
            {
                return JsonSerializer.Deserialize<BatchTransferDetailsResponse>(response, _jsonSerializerOptions);
            }
        }

        private async Task<string> PostAsync(GatewayConfig config, object request, string service, CancellationToken cancellation)
        {
            if (_fake)
            {
                if (service == "batchTransferStatus")
                {
                    var txnId = GetValue<string>(request, "txnId");
                    if (_requests.TryGetValue(txnId, out var entry))
                    {
                        var data = new BatchTransferStatus()
                        {
                            Status = (entry.DateTime.AddMinutes(1) <= DateTime.Now) ? "A" : "U",
                            TransactionId = txnId,
                            EnteredDate = entry.DateTime.ToPersian(DateTimeFormat),
                            AuthDate = entry.DateTime.AddSeconds(10).ToPersian(DateTimeFormat),
                            PayaChargesAmount = 12000 + (entry.Request.Items.Count * 1200),
                            SatnaChargesAmount = 0,
                            Reason = entry.Request.Reason,
                            Description = entry.Request.Description,
                            TotalAmount = entry.Request.Items.Sum(e => e.Amount),
                        };
                        return JsonSerializer.Serialize(data, _jsonSerializerOptions);
                    }
                    else
                    {
                        var data = new ValidateAccountsError() { ErrorCode = "-911", ErrorTitle = "تراکنش پیدا نشد." };
                        return JsonSerializer.Serialize(data, _jsonSerializerOptions);
                    }
                }
                else if (service == "batchTransferDetails")
                {
                    var txnId = GetValue<string>(request, "txnId");
                    if (_requests.TryGetValue(txnId, out var entry))
                    {
                        var data = new BatchTransferDetailsResponse()
                        {
                            Status = "A",
                            AccountNumber = entry.Request.AccountNumber,
                            Amount = entry.Request.Items.Sum(e => e.Amount),
                            Description = entry.Request.Description,
                            EnteredDate = entry.DateTime.ToPersian(DateTimeFormat),
                            Items = entry.Request.Items.Select(e => new BatchTransferItem()
                            {
                                Type = "پایا",
                                Status = "تایید شده",
                                AccountNumber = e.AccountNumber,
                                ShebaNumber = e.AccountNumber,
                                Amount = e.Amount,
                                Bank = "",
                                BenRemark = e.Description,
                                Date = entry.DateTime.AddSeconds(10).ToPersian(DateFormat),
                                Name = e.HolderName,
                                PaymentId = e.PayerId,
                                ReferenceNumber = e.RequestId,
                                RejectDescription = null
                            }).ToList()
                        };
                        return JsonSerializer.Serialize(data, _jsonSerializerOptions);
                    }
                }
            }

            var parameters = JsonSerializer.Serialize(request, _jsonSerializerOptions);
            var key = GenerateKey(config: config, service: service, parameters: parameters);
            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "key", key },
                { "parameters", parameters }
            });
            using var client = _httpClientFactory.CreateClient(Constants.MiddleEastHttpClientName);
            var response = await client.PostAsync($"{config.Domain.TrimEnd('/')}/serve/{config.Customer}/{service}", content, cancellation);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellation);
        }

        private static T GetValue<T>(object obj, string property)
        {
            var value = obj.GetType().GetProperty(property).GetValue(obj);
            return (T)value;
        }

        private static string GenerateKey(GatewayConfig config, string service, string parameters)
        {
            var tmp = config.Customer + service + parameters + config.Key;
            var bytes = Encoding.UTF8.GetBytes(tmp);
            var buffer = SHA256.HashData(bytes);
            return Convert.ToHexString(buffer).ToLower();
        }

        public static string ExtractHolderName(string input)
        {
            return input
                .Split('/', StringSplitOptions.TrimEntries)
                .FirstOrDefault();
        }

        private static BatchTransferItemState TranslateBatchTransferItemState(string state)
        {
            return state switch
            {
                "تایید شده" => BatchTransferItemState.Confirmed,
                "تایید نشده" => BatchTransferItemState.NotConfirmed,
                "عودت ذینفع" => BatchTransferItemState.BeneficiaryReturned,
                "در انتظار تائید شعبه" => BatchTransferItemState.AwaitingBranchApproval,
                "عدم تایید شعبه" => BatchTransferItemState.BranchNotApproved,
                "در انتظار تایید مالی" => BatchTransferItemState.AwaitingFinancialApproval,
                "عدم تایید مالی" => BatchTransferItemState.FinancialNotApproved,
                "عودت حواله ساتنا" => BatchTransferItemState.SatnaReturn,
                "عودت شده" => BatchTransferItemState.Returned,
                _ => throw new NotSupportedException($"Invalid state: [{state}]")
            };
        }

        private static AccountStatus TranslateAccountStatus(string code)
        {
            return code switch
            {
                "01" => AccountStatus.Closed,
                "02" => AccountStatus.Active,
                "03" => AccountStatus.BlockedDepositAllowed,
                "04" => AccountStatus.BlockedNoDeposit,
                "05" => AccountStatus.Dormant,
                "06" => AccountStatus.ErrorResponse,
                "07" => AccountStatus.Others,
                "08" => AccountStatus.NotFound,
                _ => AccountStatus.None
            };
        }

        private class GatewayConfig
        {
            public string Key { get; set; }
            public string Customer { get; set; }
            public string Domain { get; set; }

            public static GatewayConfig Parse(string content)
            {
                return JsonSerializer.Deserialize<GatewayConfig>(content);
            }
        }

        private class ErrorResponse
        {
            public Error Error { get; set; }
        }

        private class Error
        {
            public string Code { get; set; }
            public string Message { get; set; }
        }

        private class ValidateAccountsError
        {
            /// <summary>
            /// کد خطا
            /// </summary>
            public string ErrorCode { get; set; }
            /// <summary>
            /// شرح خطا
            /// </summary>
            public string ErrorTitle { get; set; }
        }

        private class BatchValidateResult
        {
            /// <summary>
            /// کد وضعیت حساب 
            /// <list type="bullet">
            /// <item>01: حساب بسته شده است</item>
            /// <item>02: حساب فعال است</item>
            /// <item>03: حساب مسدود با قابلیت واریز</item>
            /// <item>04: حساب مسدود بدون قابلیت واریز</item>
            /// <item>05: حساب راکد است</item>
            /// <item>06: بروز خطا در پاسخ دهی</item>
            /// <item>07: سایر موارد</item>
            /// <item>08: متقاضی فاقد حساب میباشد</item>
            /// </list>
            /// </summary>
            [JsonPropertyName("accountStatus")]
            public string AccountStatus { get; set; }
            /// <summary>
            /// شماره حساب
            /// </summary>
            [JsonPropertyName("acctNo")]
            public string AccountNumber { get; set; }
            /// <summary>
            /// نام بانک
            /// </summary>
            [JsonPropertyName("bank")]
            public string Bank { get; set; }
            /// <summary>
            /// شماره شبا/حساب/کارت
            /// </summary>
            [JsonPropertyName("dest")]
            public string Destination { get; set; }
            /// <summary>
            /// نام و نام خانوادگی صاحب حساب / وضعیت حساب
            /// </summary>
            [JsonPropertyName("title")]
            public string Title { get; set; }
            /// <summary>
            /// نوع شماره حساب
            /// </summary>
            [JsonPropertyName("type")]
            public string Type { get; set; }
        }

        private class WaitingSignaturesResponse
        {
            /// <summary>
            /// Service execution is waiting for signatures.
            /// </summary>
            public string Result { get; set; }
            /// <summary>
            /// شماره مرجع تراکنش 
            /// </summary>
            public long Reference { get; set; }
        }

        private class BatchTransferResponse
        {
            /// <summary>
            /// شماره مرجع تراکنش  
            /// در صورتی که تراکنش با موفقیت انجام شده باشد این پارامتر به عنوان خروجی برگردانده میشود
            /// </summary>
            [JsonPropertyName("txnId")]
            public string TransactionId { get; set; }
            /// <summary>
            /// کد خطا
            /// درصورتی که تراکنش موفقیت آمیز باشد 0000 برگردانده میشود
            /// </summary>
            public string ErrorCode { get; set; }
            /// <summary>
            /// شرح خطا
            /// </summary>
            public string ErrorDesc { get; set; }
            /// <summary>
            /// لیست تراکنش هایی که به دلیل وجود مشکل انجام نشدهاند
            /// </summary>
            public List<RejectedItem> RejectedList { get; set; }
        }

        private class RejectedItem
        {
            /// <summary>
            /// مبلغ رکورد
            /// </summary>
            public string Amount { get; set; }
            /// <summary>
            /// شرح خطای رکورد
            /// </summary>
            public string Error { get; set; }
            /// <summary>
            /// شناسه پرداخت رکورد
            /// </summary>
            public string PaymentId { get; set; }
            /// <summary>
            /// شماره شبا رکورد
            /// </summary>
            [JsonPropertyName("iban")]
            public string AccountNumber { get; set; }
            /// <summary>
            /// شماره رفرنس ارسالی
            /// </summary>
            public string ReferenceId { get; set; }
        }

        private class BatchTransferStatus
        {
            /// <summary>
            /// تاریخ و ساعت ایجاد تراکنش
            /// </summary>
            public string EnteredDate { get; set; }
            /// <summary>
            /// تاریخ و ساعت تائید تراکنش
            /// </summary>
            public string AuthDate { get; set; }
            /// <summary>
            /// شماره مرجع تراکنش
            /// </summary>
            [JsonPropertyName("txnId")]
            public string TransactionId { get; set; }
            /// <summary>
            /// وضعیت
            /// <list type="bullet">
            /// <item>E: زمانی که تراکنش توسط سرویس ثبت دستور پرداخت ثبت شده است</item>
            /// <item>U: در انتظار تائید</item>
            /// <item>A: تایید شده</item>
            /// <item>R: برگشت خورده</item>
            /// </list>
            /// </summary>
            public string Status { get; set; }
            /// <summary>
            /// کارمزد پایا
            /// </summary>
            public decimal PayaChargesAmount { get; set; }
            /// <summary>
            /// کارمزد ساتنا
            /// </summary>
            public decimal SatnaChargesAmount { get; set; }
            /// <summary>
            /// مبلغ کل تراکنش
            /// </summary>
            public decimal TotalAmount { get; set; }
            /// <summary>
            /// شرح تراکنش
            /// </summary>
            public string Description { get; set; }
            /// <summary>
            /// فیلد بابت
            /// </summary>
            public string Reason { get; set; }
        }

        private class BatchTransferDetailsResponse
        {
            /// <summary>
            /// شماره حساب مبدأ
            /// </summary>
            [JsonPropertyName("accountNo")]
            public string AccountNumber { get; set; }
            /// <summary>
            /// مبلغ کل تراکنش
            /// </summary>
            public decimal Amount { get; set; }
            /// <summary>
            /// تاریخ و ساعت ایجاد تراکنش
            /// </summary>
            public string EnteredDate { get; set; }
            /// <summary>
            /// وضعیت
            /// <list type="bullet">
            /// <item>E: زمانی که تراکنش توسط سرویس ثبت دستور پرداخت ثبت شده است</item>
            /// <item>U: در انتظار تائید</item>
            /// <item>A: تایید شده</item>
            /// <item>R: برگشت خورده</item>
            /// </list>
            /// </summary>
            public string Status { get; set; }
            /// <summary>
            /// شرح تراکنش
            /// </summary>
            public string Description { get; set; }
            /// <summary>
            /// آیتم های موجود در دسته
            /// </summary>
            public List<BatchTransferItem> Items { get; set; }
        }

        private class BatchTransferItem
        {
            /// <summary>
            /// نوع تراکنش: پایا، ساتنا، انتقالی
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// بانک مقصد
            /// </summary>
            public string Bank { get; set; }
            /// <summary>
            /// نام ذینفع
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// مبلغ
            /// </summary>
            public decimal Amount { get; set; }
            /// <summary>
            /// شرح تک تراکنش
            /// </summary>
            public string BenRemark { get; set; }
            /// <summary>
            /// شماره مرجع
            /// </summary>
            public string ReferenceNumber { get; set; }
            /// <summary>
            /// شناسه پرداخت
            /// </summary>
            public string PaymentId { get; set; }
            /// <summary>
            /// تاریخ تراکنش
            /// </summary>
            public string Date { get; set; }
            /// <summary>
            /// شماره حساب ذینفع
            /// </summary>
            [JsonPropertyName("accountNo")]
            public string AccountNumber { get; set; }
            /// <summary>
            /// شماره شبای ذینفع
            /// </summary>
            [JsonPropertyName("iban")]
            public string ShebaNumber { get; set; }
            /// <summary>
            /// وضعیت تک تراکنش
            /// </summary>
            public string Status { get; set; }
            /// <summary>
            /// دلیل برگشت تراکنش
            /// </summary>
            public string RejectDescription { get; set; }
        }

        private class WaitingResponse
        {
            public string Result { get; set; }
            public long Reference { get; set; }
        }

        private class BatchTransferRequestEntry
        {
            public required DateTime DateTime { get; init; }
            public required BatchTransferRequest Request { get; init; }
        }

        [GeneratedRegex("\"(error|errorCode)\":", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
        private static partial Regex ErrorRegex();
        private static bool IsErrorResponse(string response)
        {
            return ErrorRegex().IsMatch(response);
        }

        [GeneratedRegex("\"reference\":\\s+\\{", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
        private static partial Regex WaitingRegex();
        private static bool IsWaitingResponse(string response)
        {
            return WaitingRegex().IsMatch(response);
        }
    }
}
