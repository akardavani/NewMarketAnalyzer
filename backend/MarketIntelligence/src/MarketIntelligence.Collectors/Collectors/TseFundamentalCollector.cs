using MarketAnalyzer.Collector;
using MarketIntelligence.Domain.Entity;
using MarketAnalyzer.Fundamental;
using System.Text.Json;

namespace MarketIntelligence.Collectors
{
    public class TseFundamentalCollector : BaseCollector
    {
        private readonly IDataFetcher _fetcher;
        private readonly ITseTmcClient _tseTmcClient;
        private readonly FinancialReportService _reportService;

        public TseFundamentalCollector(IDataFetcher fetcher, ITseTmcClient tseTmcClient, FinancialReportService reportService)
        {
            _fetcher = fetcher;
            _tseTmcClient = tseTmcClient;
            _reportService = reportService;
        }

        public override string CollectorName => GetType().Name;

        //public TseFundamentalCollector(IDataFetcher fetcher) => _fetcher = fetcher;
        public async Task FetchData()
        {
            long insCode = 20926459161497908;

            //var clientType = await _tseTmcClient.GetClientTypeAsync(insCode);


            string reportType = "6/6/0";
            var ss = await GetFinancialReportAsync(insCode, reportType, CancellationToken.None);

        }

        public async Task<StandardFinancialData> GetFinancialReportAsync(long insCode, string reportType, CancellationToken ct)
        {
            await GetNew(insCode, reportType, ct);
            // reportType مثلا "6/6/0" برای ترازنامه یا "6/6/1" برای سود و زیان
            string url = $"https://cdn.tsetmc.com/api/Codal/GetStatementContentByInsCode/{reportType}/{insCode}";

            var response1 = await _fetcher.GetAsync<CodalStatementResponse>(url);

            var statement1 = FinancialStatementParser.Parse(response1.Pages.First());

            var text = await _fetcher.GetAsync(url);

            using var doc = JsonDocument.Parse(text.text);

            var ssss = doc.RootElement
                .GetProperty("statemetnContent")
                .GetArrayLength();

            Console.WriteLine(text.text.Length);


            var last = response1.Pages.Last();

            Console.WriteLine(last.Title);


            var result1 = await _fetcher.GetAsync(url); // استفاده از همان متد Generic که پیشنهاد دادم
            var response = await _fetcher.GetAsync<CodalReportDto>(url); // استفاده از همان متد Generic که پیشنهاد دادم

            

            var statement = response?.Statements
                .OrderByDescending(x => x.PublishDateTimeGregorian)
                .FirstOrDefault();

            if (statement == null)
            {
                return new StandardFinancialData
                {
                    InsCode = insCode
                };
            }

            //var options = new JsonSerializerOptions
            //{
            //    PropertyNameCaseInsensitive = true
            //};

            //RootObject result22 = JsonSerializer.Deserialize<RootObject>(result1.text, options);

            var sssss = statement.Content.Substring(0, 500);
            //var sss = new FinancialMapper().Normalize(insCode, statement);

            //string json = System.Text.Json.JsonSerializer.Serialize(sss);

            //return new FinancialMapper().Normalize(insCode, statement);

            if (response?.Statements == null ||
            !response.Statements.Any())
            {
                return null;
            }

            // جدیدترین گزارش
            var latest =
                response.Statements
                    .OrderByDescending(x =>
                        x.PublishDateTimeGregorian)
                    .First();

            // parse + normalize
            var financialData =
                _reportService.Process(
                    insCode,
                    latest);


            return new StandardFinancialData();

        }

        public async Task GetNew(long insCode, string reportType, CancellationToken ct)
        {

            //string url = $"https://cdn.tsetmc.com/api/Codal/GetStatementContentByInsCode/{reportType}/{insCode}";
            string url = $"https://cdn.tsetmc.com/api/Codal/GetStatementContentByInsCode/6/6/0/38628771709301941";


            var response = await _fetcher.GetAsync<CodalStatementResponse>(url);
            var page = response.Pages.First();
            var reader = new CodalXmlReader();

            CodalXmlDocument document =
                reader.Read(page.Content);

            var normalizer = new GridNormalizer();

            GridTable table =
                normalizer.Normalize(document.Tables.First());

            var builder = new StatementBuilder();

            StatementDocument statement =
                builder.Build(page, table);
        }

    }



    //public class RootObject
    //{
    //    public List<StatementContent> statemetnContent { get; set; }
    //}

    //public class StatementContent
    //{
    //    public string title { get; set; }
    //    public DateTime sentDateTime_Gregorian { get; set; }
    //    public DateTime publishDateTime_Gregorian { get; set; }
    //    public int publishDateTime_DEven { get; set; }
    //    public int reportSubType { get; set; }
    //    public int pageID { get; set; }
    //    public string content { get; set; } // این XML هست
    //}

}



