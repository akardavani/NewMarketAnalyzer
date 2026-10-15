using MarketAnalyzer.Domain;
using MarketAnalyzer.SharedKernel;
using System.Linq.Expressions;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    public class InvestmentFundService : IInvestmentFundService
    {
        private readonly IRepository<InvestmentFundType> _investmentFundTypeRepository;
        private readonly IRepository<InvestmentFund> _investmentFundRepository;
        private readonly IRepository<FundNavGap> _fundNavGapRepository;
        private readonly IRepository<InvestmentFundChart> _investmentFundChartRepository;

        //public override string MarketName => MarketTypeName.InvestmentFund;

        public InvestmentFundService(
            IRepository<InvestmentFundType> investmentFundTypeRepository,
            IRepository<InvestmentFund> investmentFundRepository,
            IRepository<FundNavGap> fundNavGapRepository,
            IRepository<InvestmentFundChart> investmentFundChartRepository
            )
        {
            _investmentFundTypeRepository = investmentFundTypeRepository;
            _investmentFundRepository = investmentFundRepository;
            _fundNavGapRepository = fundNavGapRepository;
            _investmentFundChartRepository = investmentFundChartRepository;
        }

        /// <summary>
        /// گرفتن دیتای صندوق ها 
        /// </summary>

        public async Task<List<InvestmentFund>> GetInvestmentFundAsync(
            Expression<Func<InvestmentFund, bool>>? filter,
            CancellationToken cancellation)
        {
            var entities = await _investmentFundRepository.GetAllAsync(filter, cancellation);
            return entities.ToList();
        }

        /// <summary>
        /// گرفتن داینامیک کندل ها
        /// </summary>
        /// <param name="filter">ساخت کد داینامیک</param>
        public async Task<List<MarketCandle>> GetCandlesAsync(
            Expression<Func<InvestmentFundChart, bool>>? filter,
            CancellationToken cancellation)
        {
            var charts = await _investmentFundChartRepository.GetAllAsync(filter, cancellation);
            var gaps = await _fundNavGapRepository.GetAllAsync(cancellation);

            var result = new List<MarketCandle>();

            // گروه‌بندی بر اساس صندوق
            var symbols = charts.GroupBy(c => c.RegNo);

            foreach (var symbolGroup in symbols)
            {
                var regNo = symbolGroup.Key;
                var ordered = symbolGroup.OrderBy(c => c.Date).ToList();

                // گپ‌های مربوط به همین صندوق
                var symbolGaps = gaps
                    .Where(g => g.RegNo == regNo)
                    .OrderBy(g => g.Date)
                    .ToList();

                decimal cumulativeFactor = 1;

                int gapIndex = 0;

                foreach (var candle in ordered)
                {
                    // اگر به تاریخ گپ رسیدیم، ضریب رو آپدیت کن
                    while (gapIndex < symbolGaps.Count && candle.Date >= symbolGaps[gapIndex].Date)
                    {
                        cumulativeFactor *= symbolGaps[gapIndex].Ratio;
                        gapIndex++;
                    }

                    result.Add(new MarketCandle
                    {
                        Id = candle.Id,
                        InstrumentId = candle.FundInstrumentId,
                        Date = candle.Date,
                        PersianDate = candle.Date.ToPersian(),
                        Close = candle.CancellationNav * cumulativeFactor,
                        Open = candle.CancellationNav * cumulativeFactor,
                        High = candle.CancellationNav * cumulativeFactor,
                        Low = candle.CancellationNav * cumulativeFactor,
                        MarketType = (int)MarketType.InvestmentFund,
                        Timeframe = (int)Timeframe.Daily,
                        NetValue = 0,
                        Timestamp = 0,
                        Volume = 0
                    });
                }
            }

            return result;


            //return entities.Select(e => new MarketCandle
            //{
            //    Id = e.Id,
            //    InstrumentId = e.FundInstrumentId,
            //    Close = e.CancellationNav,
            //    Open = e.CancellationNav,
            //    High = e.CancellationNav,
            //    Low = e.CancellationNav,
            //    PersianDate = e.Date.ToPersian(),
            //    Date = e.Date,
            //    MarketType = (int)MarketTypeDto.InvestmentFund,
            //    Timeframe = (int)TimeframeDto.Daily,
            //    NetValue = 0,
            //    Timestamp = 0,
            //    Volume = 0
            //}).ToList();
        }

        /// <summary>
        /// ذخیره انواع صندوق 
        /// </summary>
        public async Task SaveInvestmentFundTypeAsync(List<InvestmentFundType> data, CancellationToken cancellation)
        {
            await _investmentFundTypeRepository.AddRangeAsync(data, cancellation);
        }

        /// <summary>
        /// ذخیره انواع صندوق 
        /// </summary>
        //public async Task SaveInvestmentFundAsync(List<InvestmentFund> data, CancellationToken cancellation)
        //{
        //    await _investmentFundRepository.AddRangeAsync(data, cancellation);
        //}

        /// <summary>
        /// ذخیره انواع صندوق 
        /// </summary>
        public async Task SaveInvestmentFundAsync(List<InvestmentFund> data, CancellationToken cancellation)
        {
            if (data == null || data.Count == 0)
                return;

            const int chunkSize = 10; // اندازه chunk برای insert

            // 1️⃣ گرفتن تمام InstrumentId هایی که قبلاً وجود دارند
            var allIds = data.Select(d => d.RegNo).ToList();

            var existingIdsSet = (await _investmentFundRepository
                .WhereAsync(x => allIds.Contains(x.RegNo), cancellation))
                .Select(x => x.RegNo)
                .ToHashSet();

            // 2️⃣ فیلتر رکوردهای جدید
            var newItems = data.Where(x => !existingIdsSet.Contains(x.RegNo)).ToList();

            if (!newItems.Any())
                return;

            // 3️⃣ اضافه کردن chunk به chunk
            for (int i = 0; i < newItems.Count; i += chunkSize)
            {
                var chunk = newItems.Skip(i).Take(chunkSize).ToList();
                try
                {
                    await _investmentFundRepository.AddRangeAsync(chunk, cancellation);
                }
                catch (Exception ex)
                {

                    throw;
                }


                // اگر UnitOfWork داری یا SaveChangesAsync نیاز است، اینجا بزنی
                // await _dbContext.SaveChangesAsync(cancellation);
            }
        }

        /// <summary>
        /// قیمت صدور و ابطال صندوق 
        /// </summary>
        public async Task SaveInvestmentFundChartAsync(List<InvestmentFundChart> data, CancellationToken cancellation)
        {
            if (data == null || data.Count == 0)
                return;

            const int chunkSize = 500; // اندازه chunk برای insert

            // 1️⃣ گرفتن تمام InstrumentId هایی که قبلاً وجود دارند
            var allIds = data.Select(d => d.RegNo).ToList();

            var existingIdsSet = (await _investmentFundChartRepository
                .WhereAsync(x => allIds.Contains(x.RegNo), cancellation))
                .Select(x => x.RegNo)
                .ToHashSet();

            // 2️⃣ فیلتر رکوردهای جدید
            var newItems = data.Where(x => !existingIdsSet.Contains(x.RegNo)).ToList();

            if (!newItems.Any())
                return;

            // 3️⃣ اضافه کردن chunk به chunk
            for (int i = 0; i < newItems.Count; i += chunkSize)
            {
                var chunk = newItems.Skip(i).Take(chunkSize).ToList();
                await _investmentFundChartRepository.AddRangeAsync(chunk, cancellation);

                // اگر UnitOfWork داری یا SaveChangesAsync نیاز است، اینجا بزنی
                // await _dbContext.SaveChangesAsync(cancellation);
            }
        }

        /// <summary>
        /// Nav محاسبه گپ
        /// </summary>
        public async Task CalculateFundNavGap(CancellationToken cancellation)
        {
            var investmentFundCharts = await _investmentFundChartRepository.GetAllAsync(cancellation);

            // همه نمادهای صندوق‌ها
            var symbols = investmentFundCharts
                .GroupBy(e => e.RegNo)
                .Select(g => g.Key)
                .ToList();

            var gaps = new List<FundNavGap>();

            var existingData = await _fundNavGapRepository.GetAllAsync(cancellation);

            foreach (var symbol in symbols)
            {

                // داده‌های مربوط به یک صندوق خاص
                var ordered = investmentFundCharts
                    .Where(c => c.RegNo == symbol)
                    .OrderBy(c => c.Date)
                    .ToList();

                for (int i = 1; i < ordered.Count; i++)
                {
                    var prev = ordered[i - 1];
                    var curr = ordered[i];

                    var ratio = curr.CancellationNav == 0
                        ? 0
                        : prev.CancellationNav / curr.CancellationNav;

                    if (ratio < 0.02m || ratio > 20m) // شرط گپ
                    {
                        // چک کن اگر قبلاً ثبت شده نباشه
                        bool exists = existingData.Any(g => g.RegNo == curr.RegNo && g.Date == curr.Date);
                        if (!exists)
                        {
                            gaps.Add(new FundNavGap
                            {
                                FundInstrumentId = curr.FundInstrumentId,
                                RegNo = curr.RegNo,
                                Date = curr.Date,
                                PreviousNav = prev.CancellationNav,
                                CurrentNav = curr.CancellationNav,
                                Ratio = ratio
                            });
                        }
                    }
                }
            }

            // فقط رکوردهای جدید ذخیره میشن
            if (gaps.Any())
            {
                await _fundNavGapRepository.AddRangeAsync(gaps, cancellation);
            }
        }

        public async Task DeleteDuplicateDataAsync(CancellationToken cancellation)
        {
            var data = await _investmentFundChartRepository
                .GetAllAsync(null, cancellation);

            var duplicateData = data
                .GroupBy(c => new { c.RegNo, c.Date })
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .ToList();

            if (duplicateData?.Count > 0)
            {
                await _investmentFundChartRepository.DeleteRangeAsync(duplicateData);
            }
        }
    }
}
