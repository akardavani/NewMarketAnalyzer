using Microsoft.Extensions.Logging;
using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.Messaging.Asiatech
{
    internal partial class AsiatechClient
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AsiatechOption _option;
        private readonly WebHeaderCollection _defaultHeaders;

        private readonly List<Tuple<int, string, string>> _errorCodes = new List<Tuple<int, string, string>>
        {
            new Tuple<int, string, string>(0, "Send Error", "خطا در ارسال"),
            new Tuple<int, string, string>(-1, "Not Enough Credit", "موجودی کافی نیست"),
            new Tuple<int, string, string>(-2, "Server Error", "خطا در سرور"),
            new Tuple<int, string, string>(-3, "DE ACTIVE Account", "حساب کاربری غیرفعال است"),
            new Tuple<int, string, string>(-4, "Expired Account", "حساب کاربری منقضی شده است"),
            new Tuple<int, string, string>(-5, "Invalid Username or Password", "نام کاربری یا گذرواژه اشتباه است"),
            new Tuple<int, string, string>(-6, "Authentication Failure", "خطا در احراز هویت"),
            new Tuple<int, string, string>(-7, "Server Busy", "سرور مشغول است"),
            new Tuple<int, string, string>(-8, "Number At Backlist", "شماره گیرنده در لیست سیاه است"),
            new Tuple<int, string, string>(-9, "Limited In Send Day", "محدودیت در ارسال روزانه"),
            new Tuple<int, string, string>(-10, "Limited In Volume", "محدودیت در سرعت ارسال"),
            new Tuple<int, string, string>(-11, "Invalid Sender Number", "شماره ارسال کننده معتبر نمی باشد"),
            new Tuple<int, string, string>(-12, "Invalid Receiver Number", "شماره دریافت کننده معتبر نمی باشد"),
            new Tuple<int, string, string>(-13, "Invalid Destination Network", "اپراتور گیرنده معتبر نمی باشد"),
            new Tuple<int, string, string>(-14, "Unreachable Network", "شبکه غیرقابل دسترس"),
            new Tuple<int, string, string>(-15, "DE ACTIVE Sender Number", "شماره ارسال کننده غیرفعال است"),
            new Tuple<int, string, string>(-16, "Invalid Format of Sender Number", "فرمت شماره ارسال کننده اشتباه است"),
            new Tuple<int, string, string>(-17, "Tariff Not Found", "تعرفه برای این ارسال وجود ندارد"),
            new Tuple<int, string, string>(-18, "Invalid Ip Address", "آی پی فرستنده معتبر نمی باشد"),
            new Tuple<int, string, string>(-19, "Invalid Pattern", "الگوی ارسال معتبر نمی باشد"),
            new Tuple<int, string, string>(-20, "Expired Sender Number", "شماره ارسال کننده منقضی شده است"),
            new Tuple<int, string, string>(-21, "Message Contains Link", "محتوای پیام حاوی لینک است"),
            new Tuple<int, string, string>(1, "Delivered", "پیامک با موفقیت ارسال و توسط گوشی دریافت شد"),
            new Tuple<int, string, string>(2, "UnDelivered", "به گوشی نرسیده است"),
            new Tuple<int, string, string>(3, "Accepted", "پیامک به اپراتور ارسال شده"),
            new Tuple<int, string, string>(4, "ReceivedByUpstream", "به اپراتور رسیده"),
            new Tuple<int, string, string>(5, "Rejected", "توسط اپراتور رد شده"),
            new Tuple<int, string, string>(6, "NotReceiveByServer", "پیامک توسط سرور دریافت نشد"),
            new Tuple<int, string, string>(7, "ErrorInSending", "در ارسال پیامک خطا رخ داده است"),
            new Tuple<int, string, string>(8, "WaitingForSend", "منتظر ارسال"),
            new Tuple<int, string, string>(9, "Sent", "ارسال شده"),
            new Tuple<int, string, string>(10, "NotSent", "ارسال نشده"),
            new Tuple<int, string, string>(11, "Expired", "منقضی"),
            new Tuple<int, string, string>(12, "IsSending", "در حال ارسال"),
            new Tuple<int, string, string>(13, "IsCanceled", "کنسل شده"),
            new Tuple<int, string, string>(14, "BlackList", "لیست سیاه"),
            new Tuple<int, string, string>(15, "SmsIsFilter", "متن پیامک فیلتر میباشد"),
            new Tuple<int, string, string>(16, "Deleted", "حذف شده"),
            new Tuple<int, string, string>(17, "WaitingForConfirmation", "منتظر برای تایید"),
            new Tuple<int, string, string>(18, "NotEnoughBalance", "کمبود اعتبار"),
            new Tuple<int, string, string>(19, "IsPreparing", "درحال آماده سازی"),
            new Tuple<int, string, string>(20, "IsPreparedForSending", "آماده شده برای ارسال"),
            new Tuple<int, string, string>(21, "AccessDenied", "عدم دسترسی"),
            new Tuple<int, string, string>(22, "TextIsEmpty", "متن پیام خالی است"),
            new Tuple<int, string, string>(23, "InvalidInputFormat", "فرمت ورودی نامعتبر است"),
            new Tuple<int, string, string>(24, "InvalidUserOrPassword", "کاربر یا رمز عبور اشتباه است"),
            new Tuple<int, string, string>(25, "InvalidUsedMethod", "متد استفاده شده نامعتبر است"),
            new Tuple<int, string, string>(26, "InvalidSender", "فرستنده معتبر نیست"),
            new Tuple<int, string, string>(27, "InvalidMobile", "موبایل معتبر نیست"),
            new Tuple<int, string, string>(28, "InvalidReception", "هیچ گیرنده ای مشخص نشده است"),
            new Tuple<int, string, string>(29, "Stored", "ذخیره شد"),
            new Tuple<int, string, string>(30, "BlackListTable", "شماره در جدول بلک لیست وجود دارد"),
            new Tuple<int, string, string>(31, "GetDeliveryStatus", "تقاضا جهت دریافت آخرین وضعیت"),
            new Tuple<int, string, string>(32, "Unknown", "نامشخص"),
            new Tuple<int, string, string>(33, "Enroute", "در مسیر تحویل"),
            new Tuple<int, string, string>(34, "Undeliverable", "غیر قابل تحویل"),
            new Tuple<int, string, string>(35, "MessageQueueFull", "پر شدن صف ارسال"),
            new Tuple<int, string, string>(36, "UnreachableNetwork", "شبکه غیر قابل دسترس"),
        };

        public AsiatechClient(IHttpClientFactory httpClientFactory, AsiatechOption option, ILogger<AsiatechClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _option = option ?? throw new ArgumentNullException(nameof(AsiatechOption), "Asiatech option is null");
            _logger = logger;
            _defaultHeaders = new WebHeaderCollection
            {
                {"Accept", "application/json"},
                {"Authorization", $"{_option.Token}"},
            };
        }

        public async Task<bool> SendAsync(OutgoingMessage message, CancellationToken cancellation)
        {
            if (string.IsNullOrWhiteSpace(message.Recipient) || string.IsNullOrWhiteSpace(message.MessageBody))
                return false;

            if (message.Recipient.StartsWith("09"))
            {
                message.Recipient = $"989{message.Recipient[2..]}";
            }

            var url = _option.Url + "api/message/send";
            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", $"{_option.Token}");
            client.DefaultRequestHeaders.TransferEncoding.Add(new TransferCodingHeaderValue("gzip"));
            client.DefaultRequestHeaders.TransferEncoding.Add(new TransferCodingHeaderValue("deflate"));
            client.DefaultRequestHeaders.TransferEncoding.Add(new TransferCodingHeaderValue("chunked"));
            var request = await client.PostAsJsonAsync(url, new List<AsiatechMessage>
            {
                new AsiatechMessage
                {
                    SourceAddress = _option.SourceNumber,
                    DestinationAddress = message.Recipient,
                    MessageText = message.MessageBody
                }
            }, cancellation);

            //request.EnsureSuccessStatusCode();
            var response = await request.Content
                .ReadFromJsonAsync<AsiatechSendResponse>(cancellationToken: cancellation);
            if (response.Succeeded)
            {
                _logger.LogInformation($"Response of send sms is {JsonSerializer.Serialize(response)}");
            }
            else
            {
                _logger.LogError($"Response of send sms is {JsonSerializer.Serialize(response)}");
            }

            var succeeded = response.Succeeded && response.Data.Any() && response.ResultCode > 0;
            if (succeeded)
            {
                var data = response.Data[0];
                if (int.TryParse(data, out var number))
                {
                    succeeded = true;
                    _logger.LogError(
                        $"Response of send sms is {JsonSerializer.Serialize(response)}. " +
                        $"ErrorCode: {_errorCodes.FirstOrDefault(x => x.Item1 == number)?.Item2}");
                }
            }
            return succeeded;
        }
    }
}
