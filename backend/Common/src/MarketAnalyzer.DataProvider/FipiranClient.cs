using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MarketAnalyzer.DataProvider.FipiranModels;

namespace MarketAnalyzer.DataProvider
{
    public class FipiranClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public FipiranClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<InvestmentFund>> GetFundsAsync(CancellationToken cancellation = default)
        {
            var response = await GetAsync("https://fipiran.ir/services/fund/fundcompare", cancellation);
            if (!string.IsNullOrEmpty(response))
            {
                var result = JsonSerializer.Deserialize<FipiranApiResult<InvestmentFund>>(response, _serializerOptions);
                if (result.Status == 200)
                {
                    return result.Items;
                }
                else
                {
                    throw new Exception(result.Message);
                }
            }

            return null;
        }

        public async Task<List<FundType>> GetFundTypesAsync(CancellationToken cancellation = default)
        {
            var response = await GetAsync("https://fipiran.ir/services/fund/fundtype", cancellation);
            if (!string.IsNullOrEmpty(response))
            {
                var result = JsonSerializer.Deserialize<FipiranApiResult<FundType>>(response, _serializerOptions);
                if (result.Status == 200)
                {
                    return result.Items;
                }
                else
                {
                    throw new Exception(result.Message);
                }
            }

            return null;
        }

        public async Task<List<InvestmentFundInfo>> GetFundsInfoAsync(CancellationToken cancellation = default)
        {
            var funds = await GetFundsAsync(cancellation);
            if (funds is null || funds.Count == 0)
            {
                return null;
            }

            var informations = new List<InvestmentFundInfo>(funds.Count);
            foreach (var entry in funds)
            {
                var info = await GetFundInfoAsync(entry.RegNo, cancellation);
                if (info is not null)
                {
                    info.SmallSymbolName = entry.SmallSymbolName;
                    informations.Add(info);
                }
                await Task.Delay(300, cancellation);
            }

            return informations;
        }

        public async Task<InvestmentFundInfo> GetFundInfoAsync(string regno, CancellationToken cancellation = default)
        {
            var response = await GetAsync($"https://fipiran.ir/services/fund/getfund?regno={regno}", cancellation);
            try
            {
                if (!string.IsNullOrEmpty(response))
                {
                    var result = JsonSerializer.Deserialize<FipiranFundInfoResult>(response, _serializerOptions);
                    if (result.Status == 200)
                    {
                        return result.Item;
                    }
                }

                return null;
            }
            catch (Exception)
            {
                if (!IsNotCompleted(response))
                {
                   throw;
                }

                return null;
            }
        }

        private bool IsNotCompleted(string response)
        {
            try
            {
                var result = JsonSerializer.Deserialize<NotCompletedEntry>(response, _serializerOptions);
                return !result.IsCompleted;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private class NotCompletedEntry
        {
            public bool IsCompleted { get; set; }
        }

        private async Task<string> GetAsync(string url, CancellationToken cancellation = default)
        {
            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            var request = await client.GetAsync(url, cancellation);
            request.EnsureSuccessStatusCode();
            var response = await request.Content.ReadAsStreamAsync(cancellation);
            using var decompressor = new GZipStream(response, CompressionMode.Decompress);
            using var reader = new StreamReader(decompressor);
            return await reader.ReadToEndAsync(cancellation);
        }

        private class FipiranApiResult<T>
        {
            public int Status { get; set; }
            public string Message { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalCount { get; set; }
            public List<T> Items { get; set; }
        }

        private class FipiranFundInfoResult
        {
            public int Status { get; set; }
            public string Message { get; set; }
            public InvestmentFundInfo Item { get; set; }
        }
    }
}