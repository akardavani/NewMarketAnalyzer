using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using MarketAnalyzer.Sejam.Models;
using MarketAnalyzer.Sejam.Requests;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Sejam
{
    public class SejamClient
    {
        private string _token;
        private DateTime _tokenExpirationTime;
        private readonly SejamClientOptions _options;
        private readonly IHttpClientFactory _clientFactory;
        private static readonly SemaphoreSlim _sync = new(1, 1);
        private static readonly IReadOnlyDictionary<int, string> _errors = new Dictionary<int, string>()
        {
            { 2001, "موفقیت" },
            { 400, "BadRequest" },
            { 403, "عدم دسترسی" },
            { 4001, "شماره موبایل اشتباه است" },
            { 4002, "زمان اعتبار کد تأیید به پایان رسیده است" },
            { 4004, "ثبت نام تکمیل نشده است" },
            { 4005, "کدپیگیری اشتباه است" },
            { 4008, "حجم تصویر زیاد است" },
            { 4010, "کاربر اعتبارسنجی نشد" },
            { 4011, "LockedUser" },
            { 4031, "کد تأیید نامعتبر است" },
            { 4041, "کاربر مورد نظر یافت نشد" },
            { 4042, "سرویس تعریف نشده است" },
            { 4043, "ThirdPartyServiceNotFound" },
            { 4090, "Conflicts" },
            { 4150, "نوع فایل پشتیبانی نمی شود" },
            { 4290, "تعداد درخواست سرویس بیش از حد مجاز" },
            { 500, "خطای ناشناخته" },
            { 600, "عدم ارسال اس ام اس" },
            { 601, "پرداخت ناموفق" }
        };

        public SejamClient(IHttpClientFactory clientFactory, IOptions<SejamClientOptions> options)
        {
            _options = options.Value;
            _clientFactory = clientFactory;
        }

        #region [Geographical Divisions]

        /// <summary>
        /// سرویس دریافت اطلاعات کشور
        /// </summary>
        public async Task<SejamResponse<List<SejamRegion>>> GetCountriesAsync(CancellationToken cancellation = default)
        {
            using var client = await CreateClientAsync(true, cancellation);
            var response = await client.GetAsync($"v{_options.Version}/countries", cancellation);
            var result = await response.Content
                .ReadFromJsonAsync<SejamResponse<List<SejamRegion>>>(options: _options.SerializerOptions, cancellationToken: cancellation);

            return result;
        }

        /// <summary>
        /// سرویس دریافت اطلاعات استان
        /// </summary>
        /// <param name="countryId">شناسه کشور</param>
        public async Task<SejamResponse<List<SejamRegion>>> GetProvincesAsync(long countryId, CancellationToken cancellation = default)
        {
            using var client = await CreateClientAsync(true, cancellation);
            var response = await client.GetAsync($"v{_options.Version}/countries/{countryId}/provinces", cancellation);
            var result = await response.Content
                .ReadFromJsonAsync<SejamResponse<List<SejamRegion>>>(options: _options.SerializerOptions, cancellationToken: cancellation);

            return result;
        }

        /// <summary>
        /// سرویس دریافت اطلاعات شهر
        /// </summary>
        /// <param name="countryId">شناسه کشور</param>
        /// <param name="provinceId">شناسه استان</param>
        public async Task<SejamResponse<List<SejamRegion>>> GetCitiesAsync(long countryId, long provinceId, CancellationToken cancellation = default)
        {
            using var client = await CreateClientAsync(true, cancellation);
            var response = await client.GetAsync($"v{_options.Version}/countries/{countryId}/provinces/{provinceId}/cities", cancellation);
            var result = await response.Content
                .ReadFromJsonAsync<SejamResponse<List<SejamRegion>>>(options: _options.SerializerOptions, cancellationToken: cancellation);

            return result;
        }

        /// <summary>
        /// سرویس دریافت اطلاعات بخش
        /// </summary>
        /// <param name="countryId">شناسه کشور</param>
        /// <param name="provinceId">شناسه استان</param>
        /// <param name="cityId">شناسه شهر</param>
        public async Task<SejamResponse<List<SejamRegion>>> GetSectionsAsync(long countryId, long provinceId, long cityId, CancellationToken cancellation = default)
        {
            using var client = await CreateClientAsync(true, cancellation);
            var response = await client.GetAsync($"v{_options.Version}/countries/{countryId}/provinces/{provinceId}/cities/{cityId}/sections", cancellation);
            //var json = await response.Content.ReadAsStringAsync();
            var result = await response.Content
                .ReadFromJsonAsync<SejamResponse<List<SejamRegion>>>(options: _options.SerializerOptions, cancellationToken: cancellation);

            return result;
        }

        #endregion

        #region [Basic Data]

        /// <summary>
        /// سرویس دریافت اطلاعات شغل
        /// </summary>
        public async Task<SejamResponse<List<SejamEntry>>> GetJobsAsync(CancellationToken cancellation = default)
        {
            using var client = await CreateClientAsync(true, cancellation);
            var response = await client.GetAsync($"v{_options.Version}/jobs", cancellation);
            var result = await response.Content
                .ReadFromJsonAsync<SejamResponse<List<SejamEntry>>>(options: _options.SerializerOptions, cancellationToken: cancellation);

            return result;
        }

        /// <summary>
        /// سرویس دریافت اطلاعات بانک
        /// </summary>
        public async Task<SejamResponse<List<SejamBankEntry>>> GetBanksAsync(CancellationToken cancellation = default)
        {
            using var client = await CreateClientAsync(true, cancellation);
            var response = await client.GetAsync($"v{_options.Version}/banks", cancellation);
            var result = await response.Content
                .ReadFromJsonAsync<SejamResponse<List<SejamBankEntry>>>(options: _options.SerializerOptions, cancellationToken: cancellation);

            return result;
        }

        /// <summary>
        /// سرویس دریافت اطلاعات کارگزاریها
        /// </summary>
        public async Task<SejamResponse<List<SejamBrokerEntry>>> GetBrokersAsync(CancellationToken cancellation = default)
        {
            using var client = await CreateClientAsync(true, cancellation);
            var response = await client.GetAsync($"v{_options.Version}/financialbrokers", cancellation);
            var result = await response.Content
                .ReadFromJsonAsync<SejamResponse<List<SejamBrokerEntry>>>(options: _options.SerializerOptions, cancellationToken: cancellation);

            return result;
        }

        /// <summary>
        /// سرویس دریافت لیست مراکز منتخب احراز هویت
        /// </summary>
        public async Task<SejamResponse<List<SejamAuthenticateOffice>>> GetAuthenticateOfficesAsync(CancellationToken cancellation = default)
        {
            using var client = await CreateClientAsync(true, cancellation);
            var response = await client.GetAsync($"v{_options.Version}/authenticateoffices", cancellation);
            var result = await response.Content
                .ReadFromJsonAsync<SejamResponse<List<SejamAuthenticateOffice>>>(options: _options.SerializerOptions, cancellationToken: cancellation);

            return result;
        }

        #endregion

        #region [Kyc]

        /// <summary>
        /// با فراخوانی این متد، وضعیت ثبت نامی شخص در سجام مشخص میشود
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SejamResponse<PersonSejamStatus>> GetSejamStatusAsync(SejamStatusRequest request, CancellationToken cancellation)
        {
            using var client = await CreateClientAsync(true, cancellation);
            var response = await client.GetAsync($"v{_options.Version}/profiles/{request.UniqueIdentifier}/sejamStatus", cancellation);
            var jsonContent = await response.Content.ReadAsStringAsync(cancellation);
            var result = JsonSerializer.Deserialize<SejamResponse<PersonSejamStatus>>(jsonContent, _options.SerializerOptions);
            if (!result.Successed)
            {
                if (_errors.TryGetValue(result.Error.Code, out var message))
                {
                    result.Error.Description = message;
                }
            }
            result.JsonText = jsonContent;
            return result;
        }

        /// <summary>
        /// متد دریافت کد تأیید
        /// <para>این متد، کد ملی شخص را به عنوان پارامتر ورودی گرفته و پیامی حاوی کد تأیید را به تلفن همراه ثبتنامی فرد در سجام ارسال میکند.</para>
        /// <para>نکته: اگر قبلا توسط یک نهادی برای شخص کد تاییدی گرفته شده که هنوز منقضی نشده است، خروجی این متد خطای 409 خواهد بود</para>
        /// </summary>
        public async Task<bool> GetKycCodeAsync(KycCodeRequest request, CancellationToken cancellation = default)
        {
            using var client = await CreateClientAsync(true, cancellation);
            var response = await client.PostAsJsonAsync($"v{_options.Version}/kycOtp", request, cancellation);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// متد دریافت اطلاعات از سجام
        /// <para>با فراخوانی این متد، اطلاعات استعلامی شخص نمایش داده میشود. ورودی این متد کد ملی و کد تایید ارسال شده به موبایل شخص میباشد.</para>
        /// <para>نکته: برای فراخوانیهای دوم به بعد، میبایست فقط متد "دریافت اطلاعات از سجام" فراخوانی شود، درصورتی که کد تایید هنوز اعتبار داشته باشد</para>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<SejamResponse<SejamPerson>> GetUserProfileAsync(UserProfileRequest request, CancellationToken cancellation = default)
        {
            using var client = await CreateClientAsync(true, cancellation);
            var response = await client.GetAsync($"v{_options.Version}/servicesWithOtp/profiles/{request.UniqueIdentifier}?otp={request.Otp}", cancellation);
            if (!response.IsSuccessStatusCode)
            {
                return new SejamResponse<SejamPerson>()
                {
                    Error = new SejamError()
                    {
                        Code = (int)response.StatusCode,
                        Description = "ارتباط با سرویس دهنده برقرار نشد."
                    }
                };
            }

            var jsonContent = await response.Content.ReadAsStringAsync(cancellation);
            var result = Deserialize<SejamPerson>(jsonContent);
            result.JsonText = jsonContent;
            return result;
        }

        public SejamResponse<T> Deserialize<T>(string content)
        {
            return JsonSerializer.Deserialize<SejamResponse<T>>(content, _options.SerializerOptions);
        }

        #endregion

        #region [Internal]

        private async ValueTask<HttpClient> CreateClientAsync(bool addToken, CancellationToken cancellation)
        {
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_options.BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            if (addToken)
            {
                await EnsureTokenAsync(client, cancellation);
            }

            return client;
        }

        private async ValueTask EnsureTokenAsync(HttpClient client, CancellationToken cancellation)
        {
            await _sync.WaitAsync(cancellation);
            try
            {
                cancellation.ThrowIfCancellationRequested();
                if (string.IsNullOrEmpty(_token) || _tokenExpirationTime < DateTime.UtcNow)
                {
                    using var httpClient = await CreateClientAsync(false, cancellation);
                    var request = new TokenRequest()
                    {
                        Username = _options.Username,
                        Password = _options.Password
                    };
                    var response = await httpClient
                        .PostAsJsonAsync($"v{_options.Version}/accessToken", request, cancellation);

                    var result = await response.Content
                        .ReadFromJsonAsync<SejamResponse<TokenResponse>>(options: _options.SerializerOptions, cancellation);

                    if (result.Successed)
                    {
                        _token = result.Data.AccessToken;
                        _tokenExpirationTime = DateTime.UtcNow.Add(result.Data.Timelife);
                    }
                }

                client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
                client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Bearer {_token}");
            }
            finally
            {
                _sync.Release();
            }
        }

        #endregion
    }
}