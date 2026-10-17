using MarketIntelligence.Domain;
using MarketIntelligence.Infrastructure;
using System.Text.Json;

namespace MarketIntelligence.Application
{
    public sealed class IranQueueStrategy : IAnalysisStrategy
    {
        public string Name => StrategyName.IranQueue;

        private readonly IUnifiedSymbolRepository _unifiedSymbolRepository;

        public IranQueueStrategy(IUnifiedSymbolRepository unifiedSymbolRepository)
        {
            _unifiedSymbolRepository = unifiedSymbolRepository;
        }

        public async Task<AnalysisResult> ExecuteAsync(AnalysisRequest request, CancellationToken cancellation)
        {
            // ۱. خواندن داده‌های اکسل (فقط یکبار)
            var excelPath = request.Parameters["InputFilePath"].ToString();
            var fromRatio = Convert.ToInt32(request.Parameters["FromRatio"].ToString());
            var toRatio = Convert.ToInt32(request.Parameters["ToRatio"].ToString());
            var return1Month = Convert.ToInt32(request.Parameters["Return1Month"].ToString());

            var excelRows = ExcelReader.Read(excelPath);

            var allInstruments = await GetAllSymbols();
            var symbols = allInstruments
                .Select(e => e.SymbolKey)
                .ToList();

            var list = new List<IranQueueResult>();
            foreach (var row in excelRows)
            {
                var instrument = allInstruments
                    .FirstOrDefault(x => x.Symbol == row.Symbol);

                if (instrument == null) continue;

                var ratio = row.VolumeToday == 0 ? 0 : row.DemandVolume / row.VolumeToday;

                if (!(ratio >= fromRatio && ratio <= toRatio && row.Return1Month < return1Month))
                    continue;

                list.Add(new IranQueueResult
                {
                    Symbol = row.Symbol,
                    InstrumentId = instrument.InstrumentId,
                    InsCode = instrument.InsCode,
                    Ratio = ratio,
                    AvgVolume30 = row.AvgVolume30,
                    DemandVolume = row.DemandVolume,
                    VolumeToday = row.VolumeToday,
                    Return1Month = row.Return1Month,
                });
            }

            return new AnalysisResult
            {
                Strategy = Name,
                Data = new
                {
                    //MarketTimestamp = DateTime.Now,
                    QueueResult = list.OrderByDescending(e => e.Ratio)
                }
            };
        }

        private async Task<List<UnifiedSymbolRecord>> GetAllSymbols()
        {
            var symbols = await _unifiedSymbolRepository
                .GetAllAsync(MarketType.Tse, CancellationToken.None);

            var symbolKeys = symbols
                .Where(e => e.SymbolKey.EndsWith("0001") &&
                            e.SymbolKey.StartsWith("IRO") &&
                            e.IsActive)
                .AsEnumerable() // از اینجا به بعد LINQ در حافظه اجرا می‌شود
                .Where(e =>
                {
                    if (string.IsNullOrWhiteSpace(e.MetadataJson))
                        return false;

                    var meta = JsonSerializer.Deserialize<StockInstrumentMetadata>(e.MetadataJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return meta != null
                        //&& !string.IsNullOrEmpty(meta.InstrumentGroupId)
                        //&& meta.InstrumentGroupId.Contains("M1")
                        && meta.SubSectorCode != "6812"
                        && meta.Flow != 4;
                })
                .Distinct()
                .ToList();

            return symbolKeys;
        }
    }
}
