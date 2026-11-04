using MarketAnalyzer.MarketIntelligence.Application.Ports;

namespace MarketAnalyzer.MarketIntelligence.Application.Strategies.IranQueue;

public sealed class RunIranQueueHandler
{
    private readonly MarketDataIranEquitySymbolCatalog _symbolCatalog;

    public RunIranQueueHandler(MarketDataIranEquitySymbolCatalog symbolCatalog) => _symbolCatalog = symbolCatalog;

    public async Task<RunIranQueueResult> HandleAsync(RunIranQueueQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.InputFilePath))
            throw new ArgumentException("An input Excel file path is required.", nameof(request));
        if (request.FromRatio > request.ToRatio)
            throw new ArgumentException("The lower ratio must be less than or equal to the upper ratio.", nameof(request));

        var excelRows = ExcelReader.Read(request.InputFilePath);
        var symbols = await _symbolCatalog.GetActiveTseStocksAsync(cancellationToken);
        var results = new List<IranQueueResult>();

        foreach (var row in excelRows)
        {
            var instrument = symbols.FirstOrDefault(x => x.PersianName == row.Symbol || x.SymbolKey == row.Symbol);
            if (instrument is null)
                continue;

            var ratio = row.VolumeToday == 0 ? 0 : row.DemandVolume / row.VolumeToday;
            if (ratio < request.FromRatio || ratio > request.ToRatio || row.Return1Month >= request.Return1Month)
                continue;

            results.Add(new IranQueueResult
            {
                Symbol = row.Symbol,
                InstrumentId = instrument.SymbolKey,
                InsCode = instrument.ExternalId,
                Ratio = ratio,
                AvgVolume30 = row.AvgVolume30,
                DemandVolume = row.DemandVolume,
                VolumeToday = row.VolumeToday,
                Return1Month = row.Return1Month
            });
        }

        return new RunIranQueueResult(results.OrderByDescending(result => result.Ratio).ToArray());
    }
}
