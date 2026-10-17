//using MarketAnalyzer.Abstractions;
//using MarketAnalyzer.AnalysisStrategy;
//using MarketAnalyzer.Application.IranEquity.Dtos;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MarketAnalyzer.Application.IranEquity
//{
//    public class RunIranEquityMainQueryHandler
//    : IRequestHandler<RunIranEquityMainQuery, ApiResult<IranEquityMainResultDto>>
//    {
//        private readonly IAnalysisEngine _analysisEngine;

//        public RunIranEquityMainQueryHandler(IAnalysisEngine analysisEngine)
//        {
//            _analysisEngine = analysisEngine;
//        }

//        public async Task<ApiResult<IranEquityMainResultDto>> HandleAsync(
//            RunIranEquityMainQuery request,
//            CancellationToken cancellationToken = default)
//        {
//            var parameters = new Dictionary<string, object>();
//            if (!string.IsNullOrWhiteSpace(request.InputFilePath))
//            {
//                parameters["InputFilePath"] = request.InputFilePath;
//            }

//            var engineRequest = new AnalysisRequest
//            {
//                Strategy = "IranEquityMain",
//                //TargetDate = request.TargetDate ?? DateTime.Today,
//                Parameters = parameters
//            };

//            var analysisResult = await _analysisEngine.RunAsync(engineRequest, cancellationToken);

//            if (analysisResult == null || analysisResult.Data == null)
//            {
//                return ApiResult<IranEquityMainResultDto>.Fail("خطا در اجرای استراتژی یا خروجی خالی است.");
//            }

//            // استخراج و نگاشت داده‌ها
//            var dto = new IranEquityMainResultDto
//            {
//                MarketTimestamp = request.TargetDate ?? DateTime.Today
//            };

//            if (analysisResult.Data is IranEquityStrategyResultContainer rawContainer)
//            {
//                dto.TotalSymbolsAnalyzed = rawContainer.TotalCount;
//                dto.TopPicks = rawContainer.TopPicks.Select(p => new IranEquityTopPickDto
//                {
//                    Symbol = p.Symbol,
//                    Score = Math.Round(p.FinalScore, 2),
//                    Percentile = Math.Round(p.Percentile, 2),
//                    Tier = p.Tier,
//                    Price = p.LastPrice,
//                    Entry = p.Entry,
//                    StopLoss = p.StopLoss,
//                    TakeProfits = p.TakeProfits,
//                    DistanceToSupport = Math.Round(p.DistanceToSupport, 2),
//                    DistanceToResistance = Math.Round(p.DistanceToResistance, 2)
//                }).ToList();

//                dto.LeadingSectors = MapSectors(rawContainer.LeadingSectors);
//                dto.FullSectorReport = MapSectors(rawContainer.FullSectorReport);
//            }

//            return ApiResult<IranEquityMainResultDto>.Success(dto);
//        }

//        private static List<IranEquitySectorDto> MapSectors(IEnumerable<SectorAnalysisResult>? sectors)
//        {
//            if (sectors == null) return new();

//            return sectors.Select(s => new IranEquitySectorDto
//            {
//                Sector = s.SectorName,
//                AverageScore = Math.Round(s.AverageScore, 2),
//                MomentumScore = Math.Round(s.MomentumScore, 2),
//                TopTierCount = s.TopTierCount,
//                TotalMoneyFlow = Math.Round(s.TotalMoneyFlow, 2),
//                Status = s.Status ?? string.Empty,
//                TotalSymbols = s.Symbols?.Count ?? 0
//            }).ToList();
//        }
//    }
//}
