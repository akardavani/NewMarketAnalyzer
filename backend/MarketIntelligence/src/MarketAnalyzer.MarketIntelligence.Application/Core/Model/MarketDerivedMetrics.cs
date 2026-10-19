namespace MarketAnalyzer.MarketIntelligence.Application;

public sealed class MarketDerivedMetrics
{
    // ============================================================
    // Price Snapshot
    // ============================================================

    /// <summary>
    /// آخرین قیمت معامله‌شده سهم.
    /// اگر دیتای کندل روز جاری کامل و لحظه‌ای باشد، از آخرین کندل محاسبه می‌شود.
    /// در غیر این صورت می‌تواند از تابلوی بازار/ExcelData خوانده شود.
    /// </summary>
    public decimal LastPrice { get; set; }

    /// <summary>
    /// قیمت پایانی سهم در آخرین روز معاملاتی موجود.
    /// معمولاً از Close آخرین کندل روزانه محاسبه می‌شود.
    /// </summary>
    public decimal ClosingPrice { get; set; }

    /// <summary>
    /// اولین قیمت روز.
    /// اگر کندل روز جاری موجود باشد، از Open آخرین کندل روزانه محاسبه می‌شود.
    /// </summary>
    public decimal FirstPrice { get; set; }

    /// <summary>
    /// بالاترین قیمت روز جاری یا آخرین کندل موجود.
    /// از High آخرین کندل روزانه گرفته می‌شود.
    /// </summary>
    public decimal High { get; set; }

    /// <summary>
    /// پایین‌ترین قیمت روز جاری یا آخرین کندل موجود.
    /// از Low آخرین کندل روزانه گرفته می‌شود.
    /// </summary>
    public decimal Low { get; set; }

    /// <summary>
    /// درصد تغییر آخرین قیمت نسبت به قیمت پایانی روز قبل.
    /// برای تشخیص قدرت لحظه‌ای تقاضا مفید است.
    /// </summary>
    public decimal LastPriceChangePercent { get; set; }

    /// <summary>
    /// درصد تغییر قیمت پایانی نسبت به قیمت پایانی روز قبل.
    /// برای سنجش بازده روزانه واقعی سهم استفاده می‌شود.
    /// </summary>
    public decimal ClosingPriceChangePercent { get; set; }


    // ============================================================
    // Returns
    // ============================================================

    /// <summary>
    /// بازدهی سهم در حدود 1 ماه معاملاتی اخیر.
    /// معمولاً بر اساس 21 کندل روزانه محاسبه می‌شود.
    /// فرمول:
    /// CurrentClose / CloseNPeriodsAgo - 1
    /// </summary>
    public decimal Return1Month { get; set; }

    /// <summary>
    /// بازدهی سهم در حدود 3 ماه معاملاتی اخیر.
    /// معمولاً بر اساس 63 کندل روزانه محاسبه می‌شود.
    /// </summary>
    public decimal Return3Month { get; set; }

    /// <summary>
    /// بازدهی سهم در حدود 6 ماه معاملاتی اخیر.
    /// معمولاً بر اساس 126 کندل روزانه محاسبه می‌شود.
    /// </summary>
    public decimal Return6Month { get; set; }

    /// <summary>
    /// بازدهی سهم در حدود 1 سال معاملاتی اخیر.
    /// معمولاً بر اساس 252 کندل روزانه محاسبه می‌شود.
    /// </summary>
    public decimal Return1Year { get; set; }


    // ============================================================
    // Moving Averages
    // ============================================================

    /// <summary>
    /// میانگین متحرک ساده 20 روزه قیمت پایانی.
    /// برای تشخیص روند کوتاه‌مدت استفاده می‌شود.
    /// </summary>
    public decimal SMA20 { get; set; }

    /// <summary>
    /// میانگین متحرک ساده 50 روزه قیمت پایانی.
    /// برای تشخیص روند میان‌مدت استفاده می‌شود.
    /// </summary>
    public decimal SMA50 { get; set; }

    /// <summary>
    /// میانگین متحرک ساده 200 روزه قیمت پایانی.
    /// برای تشخیص روند بلندمدت و وضعیت کلی سهم استفاده می‌شود.
    /// </summary>
    public decimal SMA200 { get; set; }

    /// <summary>
    /// فاصله درصدی قیمت فعلی تا SMA20.
    /// مثبت بودن آن یعنی قیمت بالاتر از میانگین 20 روزه است.
    /// </summary>
    public decimal DistanceToSMA20 { get; set; }

    /// <summary>
    /// فاصله درصدی قیمت فعلی تا SMA50.
    /// برای فهمیدن اینکه سهم چقدر از روند میان‌مدت فاصله گرفته استفاده می‌شود.
    /// </summary>
    public decimal DistanceToSMA50 { get; set; }

    /// <summary>
    /// فاصله درصدی قیمت فعلی تا SMA200.
    /// برای سنجش ارزندگی/کشیدگی تکنیکال نسبت به روند بلندمدت استفاده می‌شود.
    /// </summary>
    public decimal DistanceToSMA200 { get; set; }


    // ============================================================
    // Momentum Indicators
    // ============================================================

    /// <summary>
    /// شاخص قدرت نسبی RSI با دوره 14.
    /// مقدار زیر 30 معمولاً اشباع فروش و مقدار بالای 70 اشباع خرید تلقی می‌شود.
    /// البته در روندهای قوی، RSI می‌تواند مدت طولانی در نواحی بالا یا پایین بماند.
    /// </summary>
    public decimal RSI14 { get; set; }

    /// <summary>
    /// شاخص جریان پول MFI با دوره 14.
    /// مشابه RSI است اما حجم معاملات را هم وارد محاسبه می‌کند.
    /// برای تشخیص ورود/خروج پول در کنار قیمت مفید است.
    /// </summary>
    public decimal MFI14 { get; set; }


    // ============================================================
    // Volume Metrics
    // ============================================================

    /// <summary>
    /// حجم معاملات آخرین روز معاملاتی موجود.
    /// از Volume آخرین کندل روزانه یا دیتای تابلو محاسبه می‌شود.
    /// </summary>
    public decimal VolumeToday { get; set; }

    /// <summary>
    /// میانگین حجم معاملات 20 روز اخیر.
    /// برای تشخیص حجم غیرعادی کوتاه‌مدت مفید است.
    /// </summary>
    public decimal AvgVolume20 { get; set; }

    /// <summary>
    /// میانگین حجم معاملات 30 روز اخیر.
    /// جایگزین محاسباتی AvgVolume30 اکسل.
    /// </summary>
    public decimal AvgVolume30 { get; set; }

    /// <summary>
    /// میانگین حجم معاملات 90 روز اخیر.
    /// برای سنجش حجم نرمال میان‌مدت سهم استفاده می‌شود.
    /// </summary>
    public decimal AvgVolume90 { get; set; }

    /// <summary>
    /// نسبت حجم امروز به میانگین حجم 20 روزه.
    /// عدد بالاتر از 1 یعنی حجم امروز بیشتر از میانگین اخیر است.
    /// در استراتژی‌های برگشتی، مقدار بالای 1.5 یا 2 می‌تواند نشانه ورود توجه بازار باشد.
    /// </summary>
    public decimal VolumeRatio20 { get; set; }

    /// <summary>
    /// نسبت حجم امروز به میانگین حجم 30 روزه.
    /// برای تشخیص افزایش حجم نسبت به رفتار معمول ماه اخیر سهم.
    /// </summary>
    public decimal VolumeRatio30 { get; set; }

    /// <summary>
    /// نسبت حجم امروز به میانگین حجم 90 روزه.
    /// کمک می‌کند بفهمیم حجم امروز نسبت به میانگین بلندتر چقدر غیرعادی است.
    /// </summary>
    public decimal VolumeRatio90 { get; set; }

    /// <summary>
    /// امتیاز گسترش حجم.
    /// می‌تواند بر اساس VolumeRatio20/30/90 ساخته شود.
    /// مقدار بالاتر یعنی حجم فعلی سهم نسبت به میانگین‌های گذشته قابل توجه‌تر است.
    /// </summary>
    public decimal VolumeExpansionScore { get; set; }


    // ============================================================
    // Liquidity / Trading Board Metrics
    // ============================================================

    /// <summary>
    /// نسبت حجم تقاضا به حجم معامله‌شده امروز.
    /// عدد بالا می‌تواند نشانه تقاضای قوی یا صف خرید باشد.
    /// این متریک از دیتای تابلو محاسبه می‌شود، نه از کندل‌ها.
    /// </summary>
    public decimal DemandToTradeRatio { get; set; }

    /// <summary>
    /// قدرت صف یا نسبت ارزش تقاضا به ارزش عرضه.
    /// اگر مقدار بسیار بالا باشد، یعنی سمت تقاضا نسبت به عرضه قوی‌تر است.
    /// از DemandVolume, DemandPrice, SupplyVolume, SupplyPrice محاسبه می‌شود.
    /// </summary>
    public decimal QueuePower { get; set; }

    /// <summary>
    /// جریان پول حقیقی.
    /// به شکل ساده:
    /// (RealBuy - RealSell) / VolumeToday
    /// عدد مثبت یعنی حقیقی‌ها خریدار خالص بوده‌اند.
    /// </summary>
    public decimal RealMoneyFlow { get; set; }

    /// <summary>
    /// امتیاز تجمیع یا جمع‌آوری سهم.
    /// بر اساس ترکیبی از خرید حقیقی، کاهش فروش حقیقی، برتری تقاضا و مثبت بودن قیمت ساخته می‌شود.
    /// این عدد لزوماً یک اندیکاتور کلاسیک نیست، بلکه امتیاز داخلی سیستم است.
    /// </summary>
    public decimal AccumulationScore { get; set; }


    // ============================================================
    // Support / Resistance / Position
    // ============================================================

    /// <summary>
    /// فاصله درصدی قیمت فعلی تا نزدیک‌ترین حمایت معتبر.
    /// اگر مقدار کم باشد، سهم نزدیک حمایت قرار دارد.
    /// معمولاً از کندل‌های ماهانه یا نواحی برگشتی مهم محاسبه می‌شود.
    /// </summary>
    public decimal? DistanceToSupport { get; set; }

    /// <summary>
    /// فاصله درصدی قیمت فعلی تا نزدیک‌ترین مقاومت معتبر.
    /// اگر مقدار کم باشد، سهم نزدیک مقاومت است و ریسک عرضه بیشتر می‌شود.
    /// </summary>
    public decimal? DistanceToResistance { get; set; }

    /// <summary>
    /// فاصله درصدی قیمت فعلی تا سقف 52 هفته اخیر.
    /// عدد منفی یا مثبت بودن تابع فرمول پیاده‌سازی است،
    /// اما بهتر است به شکل درصد فاصله از سقف ذخیره شود.
    /// </summary>
    public decimal DistanceTo52WeekHigh { get; set; }

    /// <summary>
    /// فاصله درصدی قیمت فعلی تا کف 52 هفته اخیر.
    /// برای تشخیص نزدیکی سهم به کف سالانه استفاده می‌شود.
    /// </summary>
    public decimal DistanceTo52WeekLow { get; set; }

    /// <summary>
    /// فاصله درصدی قیمت فعلی تا کف 20 روزه.
    /// برای تشخیص نزدیکی سهم به کف کوتاه‌مدت/ماهانه استفاده می‌شود.
    /// </summary>
    public decimal DistanceTo20DayLow { get; set; }

    /// <summary>
    /// فاصله درصدی قیمت فعلی تا کف 60 روزه.
    /// برای استراتژی برگشت از کف‌های رفتاری بسیار مهم است.
    /// </summary>
    public decimal DistanceTo60DayLow { get; set; }

    /// <summary>
    /// فاصله درصدی قیمت فعلی تا کف 120 روزه.
    /// برای تشخیص نزدیکی سهم به کف میان‌مدت استفاده می‌شود.
    /// </summary>
    public decimal DistanceTo120DayLow { get; set; }

    /// <summary>
    /// فاصله درصدی قیمت فعلی تا سقف 20 روزه.
    /// برای تشخیص شکست کوتاه‌مدت یا نزدیکی به مقاومت ماهانه استفاده می‌شود.
    /// </summary>
    public decimal DistanceTo20DayHigh { get; set; }

    /// <summary>
    /// فاصله درصدی قیمت فعلی تا سقف 60 روزه.
    /// برای تشخیص موقعیت سهم نسبت به سقف فصلی استفاده می‌شود.
    /// </summary>
    public decimal DistanceTo60DayHigh { get; set; }

    /// <summary>
    /// فاصله درصدی قیمت فعلی تا سقف 120 روزه.
    /// برای فهمیدن اینکه سهم چقدر از سقف میان‌مدت فاصله دارد.
    /// </summary>
    public decimal DistanceTo120DayHigh { get; set; }


    // ============================================================
    // Volatility / Risk
    // ============================================================

    /// <summary>
    /// میانگین دامنه واقعی 14 روزه.
    /// ATR14 نشان می‌دهد سهم به طور میانگین چقدر نوسان روزانه دارد.
    /// برای تعیین حد ضرر و اندازه موقعیت کاربردی است.
    /// </summary>
    public decimal ATR14 { get; set; }

    /// <summary>
    /// ATR14 به درصد قیمت فعلی.
    /// برای مقایسه نوسان‌پذیری سهم‌های مختلف بهتر از ATR خام است.
    /// </summary>
    public decimal ATR14Percent { get; set; }

    /// <summary>
    /// نوسان 30 روزه سهم، معمولاً بر اساس انحراف معیار بازده‌های روزانه.
    /// عدد بالاتر یعنی ریسک و نوسان بیشتر.
    /// </summary>
    public decimal Volatility30 { get; set; }

    /// <summary>
    /// شاخص شبه شارپ.
    /// نسبت بازده به نوسان را به شکل ساده اندازه‌گیری می‌کند.
    /// برای مقایسه کیفیت حرکت سهم‌ها استفاده می‌شود.
    /// </summary>
    public decimal SharpeLike { get; set; }

    /// <summary>
    /// بتای سهم نسبت به شاخص بازار.
    /// بتای بالاتر از 1 یعنی سهم معمولاً پرنوسان‌تر از بازار حرکت می‌کند.
    /// بتای کمتر از 1 یعنی سهم دفاعی‌تر یا کم‌نوسان‌تر است.
    /// </summary>
    public decimal BetaToMarket { get; set; }


    // ============================================================
    // Relative Strength
    // ============================================================

    /// <summary>
    /// امتیاز قدرت نسبی سهم نسبت به بازار.
    /// می‌تواند ترکیبی از قدرت نسبی 30، 60 و 90 روزه باشد.
    /// عدد بالاتر یعنی سهم نسبت به بازار عملکرد بهتری داشته است.
    /// </summary>
    public decimal RelativeStrengthPercentile { get; set; }

    /// <summary>
    /// قدرت نسبی 30 روزه سهم نسبت به شاخص کل/شاخص بازار.
    /// مثبت بودن آن یعنی سهم در 30 روز اخیر بهتر از بازار عمل کرده است.
    /// </summary>
    public decimal RelativeStrength30Market { get; set; }

    /// <summary>
    /// قدرت نسبی 60 روزه سهم نسبت به شاخص کل/شاخص بازار.
    /// </summary>
    public decimal RelativeStrength60Market { get; set; }

    /// <summary>
    /// قدرت نسبی 90 روزه سهم نسبت به شاخص کل/شاخص بازار.
    /// </summary>
    public decimal RelativeStrength90Market { get; set; }

    /// <summary>
    /// قدرت نسبی 30 روزه سهم نسبت به شاخص صنعت خودش.
    /// مثبت بودن آن یعنی سهم از صنعت خودش بهتر عمل کرده است.
    /// </summary>
    public decimal RelativeStrength30Industry { get; set; }

    /// <summary>
    /// قدرت نسبی 60 روزه سهم نسبت به شاخص صنعت خودش.
    /// </summary>
    public decimal RelativeStrength60Industry { get; set; }

    /// <summary>
    /// قدرت نسبی 90 روزه سهم نسبت به شاخص صنعت خودش.
    /// </summary>
    public decimal RelativeStrength90Industry { get; set; }


    // ============================================================
    // Trend
    // ============================================================

    /// <summary>
    /// قدرت روند سهم.
    /// می‌تواند بر اساس جایگاه قیمت نسبت به SMA20/SMA50/SMA200 و شیب میانگین‌ها ساخته شود.
    /// عدد بالاتر یعنی روند صعودی قوی‌تر.
    /// </summary>
    public decimal TrendStrength { get; set; }

    /// <summary>
    /// وضعیت روند کوتاه‌مدت.
    /// معمولاً بر اساس قیمت نسبت به SMA20 و شیب SMA20 تعیین می‌شود.
    /// مقدار پیشنهادی: -1 نزولی، 0 خنثی، 1 صعودی.
    /// </summary>
    public int ShortTermTrendDirection { get; set; }

    /// <summary>
    /// وضعیت روند میان‌مدت.
    /// معمولاً بر اساس قیمت نسبت به SMA50 و رابطه SMA20/SMA50 تعیین می‌شود.
    /// مقدار پیشنهادی: -1 نزولی، 0 خنثی، 1 صعودی.
    /// </summary>
    public int MidTermTrendDirection { get; set; }

    /// <summary>
    /// وضعیت روند بلندمدت.
    /// معمولاً بر اساس قیمت نسبت به SMA200 و رابطه SMA50/SMA200 تعیین می‌شود.
    /// مقدار پیشنهادی: -1 نزولی، 0 خنثی، 1 صعودی.
    /// </summary>
    public int LongTermTrendDirection { get; set; }


    // ============================================================
    // Candle / Reversal Signals
    // ============================================================

    /// <summary>
    /// نشان می‌دهد کندل آخر مثبت بسته شده یا نه.
    /// کندل مثبت یعنی Close بالاتر از Open باشد.
    /// </summary>
    public bool IsBullishCandle { get; set; }

    /// <summary>
    /// نشان می‌دهد قیمت پایانی نزدیک سقف روزانه بسته شده یا نه.
    /// برای تشخیص قدرت خریدار در انتهای روز مفید است.
    /// </summary>
    public bool IsCloseNearHigh { get; set; }

    /// <summary>
    /// نسبت سایه پایینی کندل به کل دامنه کندل.
    /// عدد بالا می‌تواند نشانه جمع شدن فروش یا برگشت تقاضا از کف باشد.
    /// </summary>
    public decimal LowerShadowRatio { get; set; }

    /// <summary>
    /// نسبت بدنه کندل به کل دامنه کندل.
    /// برای تشخیص قدرت کندل استفاده می‌شود.
    /// </summary>
    public decimal BodyToRangeRatio { get; set; }

    /// <summary>
    /// امتیاز کندل برگشتی.
    /// بر اساس مثبت بودن کندل، بسته شدن نزدیک سقف، سایه پایینی و محل قرارگیری نسبت به کف ساخته می‌شود.
    /// برای استراتژی برگشت از کف بسیار کاربردی است.
    /// </summary>
    public decimal ReversalCandleScore { get; set; }


    // ============================================================
    // Industry / Sector
    // ============================================================

    /// <summary>
    /// امتیاز چرخش صنعت.
    /// نشان می‌دهد صنعت سهم در حال ورود به فاز بهتر نسبت به گذشته/بازار هست یا نه.
    /// </summary>
    public decimal SectorRotationScore { get; set; }

    /// <summary>
    /// Z-Score سهم نسبت به صنعت.
    /// نشان می‌دهد رفتار سهم نسبت به صنعت خودش چقدر غیرعادی یا متفاوت است.
    /// مثبت بودن زیاد می‌تواند نشانه عملکرد بهتر از صنعت باشد.
    /// منفی بودن زیاد می‌تواند نشانه عقب‌ماندگی یا ضعف نسبت به صنعت باشد.
    /// </summary>
    public decimal ZScoreIndustry { get; set; }


    // ============================================================
    // Valuation
    // ============================================================

    /// <summary>
    /// امتیاز ارزندگی سهم.
    /// فعلاً می‌تواند بر اساس PE ساخته شود،
    /// اما بهتر است در آینده ترکیبی از PE, PB, PS, EPS Growth و حاشیه سود باشد.
    /// </summary>
    public decimal ValueScore { get; set; }

    /// <summary>
    /// ارزش منصفانه تخمینی.
    /// معمولاً از EPS و PE هدف یا میانگین تاریخی/صنعتی محاسبه می‌شود.
    /// این عدد قطعی نیست و فقط تخمین مدل است.
    /// </summary>
    public decimal EstimateFairValue { get; set; }


    public List<decimal> HistRsi { get; set; }
    public List<decimal> HistMacdLine { get; set; }
    public List<decimal> HistMacdHistogram { get; set; }
    public List<decimal> HistStochastic { get; set; }
    public List<decimal> HistCci { get; set; }
    public List<decimal> HistMomentum { get; set; }
    public List<decimal> HistObv { get; set; }
    public List<decimal> HistVwMacd { get; set; }
    public List<decimal> HistCmf { get; set; }
    public List<decimal> HistMfi { get; set; }
}

