using System.Text.Json;
using MarketAnalyzer.MarketIntelligence.Application.MarketData.Providers;
using Microsoft.Extensions.Logging;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>
    /// فهرست نمادهای بورس از وب‌سرویس TSETMC. کلید استاندارد نماد = InstrumentId (مثل IRO1FOLD0001؛ در نبودش InsCode)،
    /// شناسهٔ منبع = InsCode، نام لاتین = LVal18، «نام فارسی» = نماد کوتاه (مثل «فولاد») تا جست‌وجوی کاربر روی آن کار کند،
    /// و نام کامل در MetadataJson است.
    /// <para>
    /// <b>IsActive یعنی عضو دنیای پیگیری‌شده</b>، نه «فعال بودن در منبع»: قاعدهٔ کد قدیمی کندل‌گیری حفظ شده است —
    /// کلید با IRO شروع و با 0001 تمام شود، زیرصنعت 6812 نباشد و Flow برابر 4 نباشد. بقیه (حق تقدم، اوراق و...) ذخیره می‌شوند ولی غیرفعال‌اند.
    /// </para>
    /// </summary>
    public sealed class TsetmcStockCatalogProvider : ISymbolCatalogProvider
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };

        private readonly TsetmcWebServiceClient _client;
        private readonly ILogger<TsetmcStockCatalogProvider> _logger;

        /// <summary>سازنده.</summary>
        public TsetmcStockCatalogProvider(TsetmcWebServiceClient client, ILogger<TsetmcStockCatalogProvider> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <inheritdoc />
        public ProviderDescriptor Descriptor { get; } = TseMarket.Descriptor(TseMarket.TsetmcProviderKey);

        /// <inheritdoc />
        public async Task<FetchResult<IReadOnlyList<SymbolInfo>>> FetchSymbolsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var rows = await _client.GetInstrumentsAsync(cancellationToken);
                if (rows.Count == 0)
                    return FetchResult<IReadOnlyList<SymbolInfo>>.SuccessEmpty();

                var byKey = new Dictionary<string, SymbolInfo>(StringComparer.OrdinalIgnoreCase);
                var skipped = 0;

                foreach (var row in rows)
                {
                    var insCode = XmlValue.Long(row, "InsCode");
                    if (insCode == null)
                    {
                        skipped++;
                        continue;
                    }

                    var instrumentId = XmlValue.Str(row, "InstrumentID");
                    var key = string.IsNullOrWhiteSpace(instrumentId) ? insCode.Value.ToString() : instrumentId;
                    var subSector = XmlValue.Str(row, "CSoSecVal");
                    var flow = XmlValue.Int(row, "Flow");

                    var tracked = key.StartsWith("IRO", StringComparison.Ordinal)
                                  && key.EndsWith("0001", StringComparison.Ordinal)
                                  && subSector != "6812"
                                  && flow != 4;

                    var metadata = new TsetmcStockMetadata
                    {
                        FullName = XmlValue.Persian(XmlValue.Str(row, "LVal30")),
                        CompanyName = XmlValue.Persian(XmlValue.Str(row, "LSoc30")),
                        Mnemonic = XmlValue.Str(row, "CValMne"),
                        FourDigitCompanyCode = XmlValue.Str(row, "CSocCSAC"),
                        CIsin = XmlValue.Str(row, "CIsin"),
                        InstrumentGroupId = XmlValue.Str(row, "CGrValCot"),
                        MarketSegment = XmlValue.Str(row, "YMarNSC"),
                        BoardCode = XmlValue.Long(row, "CComVal"),
                        SectorCode = XmlValue.Str(row, "CSecVal"),
                        SubSectorCode = subSector,
                        InstrumentType = XmlValue.Long(row, "YVal"),
                        SharesOrBondsNumber = XmlValue.Long(row, "ZTitad"),
                        Flow = flow,
                        Valid = XmlValue.Int(row, "Valid")
                    };

                    // یک نماد ممکن است در چند بازار جریان تکرار شود؛ آخری برنده است.
                    byKey[key] = new SymbolInfo
                    {
                        SymbolKey = key,
                        ExternalId = insCode.Value.ToString(),
                        Name = XmlValue.Str(row, "LVal18"),
                        PersianName = XmlValue.Persian(XmlValue.Str(row, "LVal18AFC")),
                        IsActive = tracked,
                        MetadataJson = JsonSerializer.Serialize(metadata, JsonOptions),
                        MetadataVersion = TsetmcStockMetadata.CurrentVersion
                    };
                }

                if (skipped > 0)
                    _logger.LogWarning("Skipped {Count} TSETMC instruments without InsCode", skipped);

                var list = byKey.Values.ToList();
                return list.Count == 0
                    ? FetchResult<IReadOnlyList<SymbolInfo>>.SuccessEmpty()
                    : FetchResult<IReadOnlyList<SymbolInfo>>.Success(list, list.Count);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "TSETMC instrument fetch failed");
                return FetchResult<IReadOnlyList<SymbolInfo>>.Failed($"TSETMC instrument fetch failed: {ex.Message}", ex);
            }
        }
    }
}
