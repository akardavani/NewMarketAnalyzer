using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketAnalyzer.MarketIntelligence.Application.IranEquity.Dtos
{
    public class IranEquityMainResultDto
    {
        public DateTime MarketTimestamp { get; set; }
        public int TotalSymbolsAnalyzed { get; set; }
        public List<IranEquityTopPickDto> TopPicks { get; set; } = new();
        public List<IranEquitySectorDto> LeadingSectors { get; set; } = new();
        public List<IranEquitySectorDto> FullSectorReport { get; set; } = new();
    }

    public class IranEquityTopPickDto
    {
        public string Symbol { get; set; } = string.Empty;
        public double Score { get; set; }
        public double Percentile { get; set; }
        public string Tier { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? Entry { get; set; }
        public decimal? StopLoss { get; set; }
        public List<decimal>? TakeProfits { get; set; }
        public double DistanceToSupport { get; set; }
        public double DistanceToResistance { get; set; }
    }

    public class IranEquitySectorDto
    {
        public string Sector { get; set; } = string.Empty;
        public decimal AverageScore { get; set; }
        public decimal MomentumScore { get; set; }
        public int TopTierCount { get; set; }
        public decimal TotalMoneyFlow { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalSymbols { get; set; }
    }
}
