using MarketAnalyzer.DataProvider.Json;
using System;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.DataProvider.TseModels
{
    public class TseFutureInformation
    {
        /// <summary>
        /// شناسه قرارداد (ID)
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        /// <summary>
        /// کد 12 رقمی شرکت (Cisin)
        /// </summary>
        [JsonPropertyName("cisin")]
        public string CompanyIsin { get; set; }
        /// <summary>
        /// شناسه شرکت نماد پایه (BaseInstrumentCIsin)
        /// </summary>
        [JsonPropertyName("baseInstrumentCIsin")]
        public string BaseInstrumentCompanyIsin { get; set; }
        /// <summary>
        /// اندازه قرارداد (Quantity)
        /// </summary>
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        /// <summary>
        /// حداکثر تعداد موقعیتهای باز مجاز توسط مشتری حقوقی (MaxClientOP)
        /// </summary>
        [JsonPropertyName("maxClientOP")]
        public long? MaxLegalClientOpenPositions { get; set; }
        /// <summary>
        /// حداکثر تعداد موقعیتهای باز مجاز توسط مشتری حقیقی (MaxActClientOP)
        /// </summary>
        [JsonPropertyName("maxActClientOP")]
        public long? MaxNaturalClientOpenPositions { get; set; }
        /// <summary>
        /// حداکثر تعداد موقعیتهای باز مجاز توسط کارگزار (MaxBrokerOP)
        /// </summary>
        [JsonPropertyName("maxBrokerOP")]
        public long MaxBrokerOpenPositions { get; set; }
        /// <summary>
        /// حداکثر تعداد موقعیتهای باز مجاز در بازار (MaxMarketOP)
        /// </summary>
        [JsonPropertyName("maxMarketOP")]
        public long MaxMarketOpenPositions { get; set; }
        /// <summary>
        /// تاریخ شروع قرارداد (BeginDate)
        /// </summary>
        [JsonPropertyName("beginDate")]
        [JsonConverter(typeof(TseJsonConveterDateOnly))]
        public DateOnly? StratDate { get; set; }
        /// <summary>
        /// تاریخ پایان قرارداد (EndDate)
        /// </summary>
        [JsonPropertyName("endDate")]
        [JsonConverter(typeof(TseJsonConveterDateOnly))]
        public DateOnly EndDate { get; set; }
        /// <summary>
        /// تاریخ تسویه فیزیکی (PhysicalSettlementDate)
        /// </summary>
        [JsonPropertyName("physicalSettlementDate")]
        [JsonConverter(typeof(TseJsonConveterDateOnly))]
        public DateOnly? PhysicalSettlementDate { get; set; }
        /// <summary>
        /// تاریخ تعیین اولویت تسویه (PrioritizationstlmntDate)
        /// </summary>
        [JsonPropertyName("prioritizationstlmntDate")]
        [JsonConverter(typeof(TseJsonConveterDateOnly))]
        public DateOnly? PrioritizationSettlementDate { get; set; }
        /// <summary>
        /// تاریخ تسویه نقدی (CashStlmntDate)
        /// </summary>
        [JsonPropertyName("cashStlmntDate")]
        [JsonConverter(typeof(TseJsonConveterDateOnly))]
        public DateOnly? CashSettlementDate { get; set; }
        /// <summary>
        /// وجه تضمین اولیه (IMAValue)
        /// </summary>
        [JsonPropertyName("imaValue")]
        public decimal InitialMarginValue { get; set; }
        /// <summary>
        /// وجه تضمین لازم (RMBValue)
        /// </summary>
        [JsonPropertyName("rmbValue")]
        public decimal RequiredMarginValue { get; set; }
        /// <summary>
        /// ضریب گرد کردن (CRound_Value)
        /// </summary>
        [JsonPropertyName("cRound_Value")]
        public long RoundingCoefficient { get; set; }
        /// <summary>
        /// وجه تضمین اضافی (DExtraMargin)
        /// </summary>
        [JsonPropertyName("dExtraMargin")]
        public long ExtraMargin { get; set; }
        /// <summary>
        /// درصد وجه تضمین حداقل (MaintananceMarginPercent)
        /// </summary>
        [JsonPropertyName("maintananceMarginPercent")]
        public decimal MaintananceMarginPercent { get; set; }
        /// <summary>
        /// تسویه بخشی از قرارداد مجاز است (PartialStlmntIsAllowded)
        /// </summary>
        [JsonPropertyName("partialStlmntIsAllowded")]
        public bool PartialSettlementAllowed { get; set; }
        /// <summary>
        /// ماه شمسی قرارداد (Month)
        /// </summary>
        [JsonPropertyName("month")]
        public int PersianMonth { get; set; }
        /// <summary>
        /// سال شمسی قرارداد (Year)
        /// </summary>
        [JsonPropertyName("year")]
        public int PersianYear { get; set; }
        /// <summary>
        /// شناسه نوع تسویه (SettlementTypeID)
        /// </summary>
        [JsonPropertyName("settlementTypeID")]
        public int SettlementTypeId { get; set; }
        /// <summary>
        /// نام نوع تسویه (SettlemnetTypeName)
        /// </summary>
        [JsonPropertyName("settlementTypeName")]
        public string SettlemnetTypeName { get; set; }
        /// <summary>
        /// شناسه نوع تخصیص (AppropiationID)
        /// </summary>
        [JsonPropertyName("appropiationID")]
        public int AppropriationId { get; set; }
        /// <summary>
        /// نوع روش تخصیص (AppropriationName)
        /// </summary>
        [JsonPropertyName("appropriationName")]
        public string AppropriationName { get; set; }
        /// <summary>
        /// شناسه تعیین قیمت دارایی پایه (BaseAssetPriceDeterminationID)
        /// </summary>
        [JsonPropertyName("baseAssetPriceDeterminationID")]
        public int BaseAssetPriceDeterminationId { get; set; }
        /// <summary>
        /// نحوه تعیین قیمت (BaseAssetPriceName)
        /// </summary>
        [JsonPropertyName("baseAssetPriceName")]
        public string BaseAssetPriceName { get; set; }
        /// <summary>
        /// نرخ خسارت (PenaltyRatio)
        /// </summary>
        [JsonPropertyName("penaltyRatio")]
        public decimal PenaltyRatio { get; set; }
        /// <summary>
        /// کارمزد تسویه فیزیکی (PhysicalFeeRatio)
        /// </summary>
        [JsonPropertyName("physicalFeeRatio")]
        public decimal PhysicalFeeRatio { get; set; }
        /// <summary>
        /// کارمزد تسویه نقدی (CashFeeRatio)
        /// </summary>
        [JsonPropertyName("cashFeeRatio")]
        public decimal CashFeeRatio { get; set; }
        /// <summary>
        /// نرخ مالیات (TaxRatio)
        /// </summary>
        [JsonPropertyName("taxRatio")]
        public decimal TaxRatio { get; set; }
        /// <summary>
        /// تعداد موقعیت های باز (MOP)
        /// </summary>
        [JsonPropertyName("mop")]
        public int? OpenPositions { get; set; }
        /// <summary>
        /// شناسه داخلی تماد فقط در حالت جیسون
        /// </summary>
        [JsonPropertyName("insCode")]
        public long? InsCode { get; set; }
        /// <summary>
        /// نام نماد پایه فقط در جیسون
        /// </summary>
        [JsonPropertyName("baseInstrumentLVal18AFC")]
        public string BaseInstrumentName { get; set; }
    }
}
