using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MarketAnalyzer.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MarketAnalyzer.ApiClient
{
    public partial class FundApiClient
    {
        public const string ClientName = "ApiClient";

        private readonly ApiClientOptions _options;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals
        };

        public FundApiClient(
          IHttpClientFactory clientFactory,
          IOptions<ApiClientOptions> options,
          IHttpContextAccessor httpContextAccessor)
        {
            _options = options.Value;
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;

            _serializerOptions.Converters.Insert(0, new DateOnlyJsonConverter());
            _serializerOptions.Converters.Insert(0, new DateTimeJsonConverter());
        }

        #region [Home]

        public async Task<ApiClientResult<PortalHomePageViewData>> GetHomePageViewDataAsync(PageViewDataRequest request, CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            var url = BuildUrl("Home/GetPageViewData", new
            {
                request.NavsFromDate,
                request.NavsToDate
            });
            return await client.GetFromJsonAsync<ApiClientResult<PortalHomePageViewData>>(url, _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<int>> ContactUsAsync(ContactusRequest request, CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            var response = await client.PostAsJsonAsync("Home/Contactus", request, _serializerOptions, cancellation);
            return await response.Content.ReadFromJsonAsync<ApiClientResult<int>>(_serializerOptions, cancellation);
        }

        #endregion

        #region [Account]

        public async Task<string> ResendOtpAsync(string userName, CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            var url = BuildUrl("Account/ResendOtp", new
            {
                UserName = userName
            });
            return await client.GetStringAsync(url, cancellation);
        }

        public async Task<ApiClientResult<CheckSejamStateResult>> CheckSejamStateAsync(CheckSejamStateRequest request, CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            var url = BuildUrl("Account/CheckSejamState", new { request.NationalCode });
            return await client.GetFromJsonAsync<ApiClientResult<CheckSejamStateResult>>(url, cancellation);
        }

        public async Task<string> ResendSejamKycAsync(string nationalCode, CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            var url = BuildUrl("Account/ResendSejamKyc", new
            {
                NationalCode = nationalCode
            });
            return await client.GetStringAsync(url, cancellation);
        }

        public async Task<ApiClientResult<SejamKycResult>> SejamKycAsync(SejamKycRequest request, CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            var url = BuildUrl("Account/SejamKyc", new { request.NationalCode, request.Otp });
            return await client.GetFromJsonAsync<ApiClientResult<SejamKycResult>>(url, _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<SejamProfileResult>> GetSejamProfileAsync(SejamProfileRequest request, CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            var url = BuildUrl("Account/GetSejamProfile", new
            {
                request.SejamProfileHistoryId,
                request.NationalCode,
                request.Otp
            });
            return await client.GetFromJsonAsync<ApiClientResult<SejamProfileResult>>(url, _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<bool?>> Signup(SignupRequest request, CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            var response = await client.PostAsJsonAsync("Account/Signup", request, cancellation);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return await response.Content.ReadFromJsonAsync<ApiClientResult<bool?>>(_serializerOptions, cancellation);
        }

        public async Task<string> ResendForgotPasswordOtpAsync(string userName, string ipAddress, CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            var url = BuildUrl("Account/ResendForgotPasswordOtp", new
            {
                UserName = userName,
                IpAddress = ipAddress
            });
            return await client.GetStringAsync(url, cancellation);
        }

        public async Task<ApiClientResult<CollectionResult<LoginHistoryResult>>> GetLoginHistoryAsync(
         int page,
         int rows,
         CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            var url = BuildUrl("Account/LoginHistory", new
            {
                Page = page,
                Rows = rows,
            });
            return await client.GetFromJsonAsync<ApiClientResult<CollectionResult<LoginHistoryResult>>>(url, _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<InvestorProfileResult>> GetInvestorProfileAsync(CancellationToken cancellation)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            return await client.GetFromJsonAsync<ApiClientResult<InvestorProfileResult>>("Account/GetInvestorProfile", _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<CheckSejamStateResult>> GetSejamKycCodeAsync(GetSejamKycCodeRequest request, CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            var url = BuildUrl("Account/GetSejamKycCode", new { request.NationalCode });
            return await client.GetFromJsonAsync<ApiClientResult<CheckSejamStateResult>>(url, _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<InvestorProfileResult>> UpdateInvestorProfileAsync(UpdateInvestorProfileRequest request, CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            var url = BuildUrl("Account/UpdateInvestorProfile", new { request.NationalCode, request.KycCode });
            return await client.GetFromJsonAsync<ApiClientResult<InvestorProfileResult>>(url, _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<bool>> GetChangePasswordOtpCodeAsync(string IpAddress, CancellationToken cancellation)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            var url = BuildUrl("v1/Account/GetChangePasswordOtpCode", new { IpAddress });
            return await client.GetFromJsonAsync<ApiClientResult<bool>>(url, _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<bool>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellation)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            var response = await client.PostAsJsonAsync("Account/ChangePassword", request, cancellation);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ApiClientResult<bool>>(_serializerOptions, cancellation);
        }

        #endregion

        #region [Fund]

        public async Task<ApiClientResult<CollectionResult<ProspectusesResult>>> GetAllProspectusesAsync(CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            return await client.GetFromJsonAsync<ApiClientResult<CollectionResult<ProspectusesResult>>>("Fund/GetAllProspectuses", _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<CollectionResult<StatutesResult>>> GetAllStatusesAsync(CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            return await client.GetFromJsonAsync<ApiClientResult<CollectionResult<StatutesResult>>>("Fund/GetAllStatuses", _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<CollectionResult<InvestmentPolicyStatementsResult>>> GetAllInvestmentPolicyStatementsAsync(CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            return await client.GetFromJsonAsync<ApiClientResult<CollectionResult<InvestmentPolicyStatementsResult>>>("Fund/GetAllInvestmentPolicyStatements", _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<CollectionResult<CompanyBankAccountsResult>>> GetAllCompanyBankAccountsAsync(CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            return await client.GetFromJsonAsync<ApiClientResult<CollectionResult<CompanyBankAccountsResult>>>("Fund/GetAllCompanyBankAccounts", _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<CollectionResult<SuperUnitsResult>>> GetAllSuperUnitsAsync(CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            return await client.GetFromJsonAsync<ApiClientResult<CollectionResult<SuperUnitsResult>>>("Fund/GetAllSuperUnits", _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<IReadOnlyCollection<PillarRolesResumeFileResult>>> GetAllPillarRolesResumeFile(CancellationToken cancellation)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            return await client.GetFromJsonAsync<ApiClientResult<IReadOnlyCollection<PillarRolesResumeFileResult>>>("Fund/GetAllPortalPillarRolesResumeFile", _serializerOptions, cancellation);
        }

        #endregion

        #region [Request]

        public async Task<ApiClientResult<CollectionResult<RequestsResult>>> GetAllRequestAsync(
         int page,
         int rows,
         DateTime? fromDate,
         DateTime? toDate,
         CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            var url = BuildUrl("Request/GetAll", new
            {
                Page = page,
                Rows = rows,
                FromDate = fromDate,
                ToDate = toDate
            });
            return await client.GetFromJsonAsync<ApiClientResult<CollectionResult<RequestsResult>>>(url, _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<InvestorCardexResult>> GetInvestorCardexAsync(
          DateOnly? fromDate,
          DateOnly? toDate,
          CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            var url = BuildUrl("Request/GetInvestorRequestCardexes", new
            {
                fromDate,
                toDate
            });
            return await client.GetFromJsonAsync<ApiClientResult<InvestorCardexResult>>(url, _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<ReportResult>> ExportInvestorCardexAsync(
          DateOnly? fromDate,
          DateOnly? toDate,
          CancellationToken cancellation)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            var url = BuildUrl("Request/ExportInvestorRequestCardexes", new
            {
                fromDate,
                toDate
            });
            return await client.GetFromJsonAsync<ApiClientResult<ReportResult>>(url, _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<long?>> CreateIssuanceRequest(CreatePortalIssuanceRequest request, CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            var response = await client.PostAsJsonAsync("Request/PortalIssue", request, cancellation);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ApiClientResult<long?>>(_serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<IssuanceRequestViewData>> GetIssuanceRequestViewDataAsync(CancellationToken cancellation)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            return await client.GetFromJsonAsync<ApiClientResult<IssuanceRequestViewData>>("Request/GetIssuanceRequestViewData", _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<long>> VerifyOnlineDepositIssuanceRequestAsync(VerifyOnlineDepositIssuanceRequestRequest request, CancellationToken cancellation)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            var response = await client.PostAsJsonAsync("Request/VerifyOnlineDepositIssuanceRequest", request, _serializerOptions, cancellation);
            return await response.Content.ReadFromJsonAsync<ApiClientResult<long>>(_serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<long?>> CreateCancellationRequest(CreatePortalCancellationRequest request, CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            var response = await client.PostAsJsonAsync("Request/PortalCancel", request, cancellation);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ApiClientResult<long?>>(_serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<CancellationRequestViewData>> GetCancellationRequestViewDataAsync(CancellationToken cancellation)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            return await client.GetFromJsonAsync<ApiClientResult<CancellationRequestViewData>>("Request/GetCancellationRequestViewData", _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<LastInvestmentCertificateResult>> GetLastInvestmentCertificate(CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            return await client.GetFromJsonAsync<ApiClientResult<LastInvestmentCertificateResult>>("Request/GetPortalLastInvestmentCertificate", _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<ReportResult>> ExportLastInvestmentCertificate(long requestId, int unitTypeId, CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            var url = BuildUrl("Request/ExportPortalLastInvestmentCertificate", new
            {
                Id = requestId,
                UnitTypeId = unitTypeId
            });
            return await client.GetFromJsonAsync<ApiClientResult<ReportResult>>(url, _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<InvestorAccountTurnoverResult>> GetInvestorAccountTurnoverAsync(
          DateOnly? fromDate,
          DateOnly? toDate,
          CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            var url = BuildUrl("Request/GetPortalInvestorAccountTurnover", new
            {
                FromDate = fromDate,
                ToDate = toDate
            });
            return await client.GetFromJsonAsync<ApiClientResult<InvestorAccountTurnoverResult>>(url, _serializerOptions, cancellation);
        }
        #endregion

        #region [OnlineDeposit]

        public async Task<ApiClientResult<long>> DepositRequestAsync(DepositRequest request, CancellationToken cancellation)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            var response = await client.PostAsJsonAsync("PaymentGateway/DepositRequest", request, _serializerOptions, cancellation);
            return await response.Content.ReadFromJsonAsync<ApiClientResult<long>>(_serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<string>> VerifyOnlineDepositAsync(VerifyOnlineDepositRequest request, CancellationToken cancellation)
        {
            using var client = CreateHttpClient(AuthenticationType.Jwt);
            var response = await client.PostAsJsonAsync("PaymentGateway/VerifyDeposit", request, _serializerOptions, cancellation);
            return await response.Content.ReadFromJsonAsync<ApiClientResult<string>>(_serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<ReceiveOnlineDepositResult>> ReceiveOnlineDepositAsync(ReceiveOnlineDepositRequest request, CancellationToken cancellation)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            var response = await client.PostAsJsonAsync("PaymentGateway/ReceiveDeposit", request, _serializerOptions, cancellation);
            return await response.Content.ReadFromJsonAsync<ApiClientResult<ReceiveOnlineDepositResult>>(_serializerOptions, cancellation);
        }

        #endregion

        #region [Report]

        public async Task<ApiClientResult<CollectionResult<DailyAssetCompositionResult>>> GetDailyAssetComposition(
         int page,
         int rows,
         DateOnly? fromDate,
         DateOnly? toDate,
         CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            var url = BuildUrl("Reports/DailyAssetsComposition", new
            {
                Page = page,
                Rows = rows,
                FromDate = fromDate,
                ToDate = toDate
            });
            return await client.GetFromJsonAsync<ApiClientResult<CollectionResult<DailyAssetCompositionResult>>>(url, _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<PeriodicAssetCompositionResult>> GetPeriodicAssetComposition(
         DateOnly? toDate,
         CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            var url = BuildUrl("Reports/PeriodicAssetComposition", new
            {
                ToDate = toDate
            });
            return await client.GetFromJsonAsync<ApiClientResult<PeriodicAssetCompositionResult>>(url, _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<PeriodicFundReturnResult>> GetPeriodicFundReturnAsync(
         DateOnly? toDate,
         CancellationToken cancellation)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            var url = BuildUrl("Reports/PeriodicFundReturn", new
            {
                ToDate = toDate
            });
            return await client.GetFromJsonAsync<ApiClientResult<PeriodicFundReturnResult>>(url, _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<CollectionResult<DailyFundReturnResult>>> GetDailyFundReturnAsync(
         int page,
         int rows,
         DateOnly? fromDate,
         DateOnly? toDate,
         CancellationToken cancellation)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            var url = BuildUrl("Reports/DailyFundReturn", new
            {
                Page = page,
                Rows = rows,
                ToDate = toDate,
                FromDate = fromDate
            });
            return await client.GetFromJsonAsync<ApiClientResult<CollectionResult<DailyFundReturnResult>>>(url, _serializerOptions, cancellation);
        }

        public async Task<ApiClientResult<List<FundStatusReturnResult>>> FundStatusReturn(
            DateOnly fromDate,
            DateOnly toDate,
            CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            var url = BuildUrl("Reports/FundStatusReturn", new
            {
                DayInterval = 1,
                FromDate = fromDate,
                ToDate = toDate
            });
            return await client.GetFromJsonAsync<ApiClientResult<List<FundStatusReturnResult>>>(url, _serializerOptions, cancellation);
        }

        #endregion

        public async Task<FileContentResult> LoadFile(string attachmentId, CancellationToken cancellation = default)
        {
            using var client = CreateHttpClient(AuthenticationType.ApiKey);
            using var message = new HttpRequestMessage(HttpMethod.Get, $"FileStore/Load?Id={attachmentId}");
            var response = await client.SendAsync(message, cancellation);
            response.EnsureSuccessStatusCode();
            string decodedFileName = Uri.UnescapeDataString(response.Headers.GetValues("X-File-Name").FirstOrDefault());
            var fileContent = await response.Content.ReadAsByteArrayAsync(cancellation);
            var fileType = response.Content.Headers.ContentType.MediaType ?? null;
            return new FileContentResult(fileContent, fileType)
            {
                FileDownloadName = decodedFileName
            };
        }

        private static string BuildUrl(string path, object data)
        {
            ArgumentNullException.ThrowIfNull(data);

            var builder = new UriBuilder(path);
            var query = HttpUtility.ParseQueryString(builder.Query);
            var properties = data.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(data)?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    query[property.Name] = value;
                }
            }
            return $"{path}?{query}";
        }

        private HttpClient CreateHttpClient(AuthenticationType authenticationType)
        {
            var client = _clientFactory.CreateClient(ClientName);
            client.BaseAddress = new Uri(_options.Url);
            if (authenticationType == AuthenticationType.Jwt)
            {
                var jwt = _httpContextAccessor.HttpContext.ExportAccessToken();
                if (string.IsNullOrEmpty(jwt))
                {
                    throw new Exception($"The jwt token not fund. The user is not authenticated.");
                }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            }
            else
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("api_key", _options.ApiKey);
            }

            return client;
        }

        private enum AuthenticationType
        {
            Jwt,
            ApiKey
        }

        internal partial class DateTimeJsonConverter : JsonConverter<DateTime>
        {
            public const int FormatLength = 10;

            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = reader.GetString();
                var match = RxDate().Match(value);
                if (match is null || !match.Success)
                {
                    throw new FormatException($"Invalid Date value: [{value}]");
                }

                var milliseconds = long.Parse(match.Groups[1].Value);
                return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).DateTime;
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value);
            }

            //^/D\((\d+)\)/$
            [GeneratedRegex(@"^/D\((\d+)\)/$", RegexOptions.Compiled)]
            private static partial Regex RxDate();
        }

        internal partial class DateOnlyJsonConverter : JsonConverter<DateOnly>
        {
            public const int FormatLength = 10;

            public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = reader.GetString();
                var match = RxDate().Match(value);
                if (match is null || !match.Success)
                {
                    throw new FormatException($"Invalid Date value: [{value}]");
                }

                var milliseconds = long.Parse(match.Groups[1].Value);
                var date = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).DateTime;
                return DateOnly.FromDateTime(date);
            }

            public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
            {
                Span<byte> buffer = stackalloc byte[FormatLength];
                var formattedSuccessfully = value.TryFormat(buffer, out int charsWritten, "O", CultureInfo.InvariantCulture);
                writer.WriteStringValue(buffer);
            }

            //^/D\((\d+)\)/$
            [GeneratedRegex(@"^/D\((\d+)\)/$", RegexOptions.Compiled)]
            private static partial Regex RxDate();
        }
    }
}
