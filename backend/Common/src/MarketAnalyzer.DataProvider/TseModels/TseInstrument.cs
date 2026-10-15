using System;

namespace MarketAnalyzer.DataProvider.TseModels
{
    public class TseInstrument
    {
        /// <summary>
        /// کد 12 رقمی لاتین نماد (InstrumentID)
        /// </summary>
        public string InstrumentId { get; set; }
        /// <summary>
        /// بورس معاملات نماد
        /// </summary>
        public DataFlow Flow { get; set; }
        /// <summary>
        /// کد 12 رقمی شرکت (CIsin)
        /// </summary>
        public string CompanyIsin { get; set; }
        /// <summary>
        /// نام 18 رقمي لاتين نماد (LVal18)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// نام 30 رقمي فارسي نماد (LVal30)
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// کد 18 رقمي فارسي نماد (LVal18AFC)
        /// </summary>
        public string NativeName { get; set; }
        /// <summary>
        /// نام 30 رقمي فارسي شرکت (LSoc30)
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// کد 4 رقمی شرکت (CSocCSAC)
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// کد 5 رقمي لاتين نماد (CValMne)
        /// </summary>
        public string Mnemonic { get; set; }
        /// <summary>
        /// نوع نماد يکي از مقادير: A - I - O (CGdSVal)
        /// </summary>
        public string InstrumentClass { get; set; }
        /// <summary>
        /// نوع نماد (YVal)
        /// </summary>
        public int InstrumentType { get; set; }
        /// <summary>
        /// کد بازار (YMarNSC)
        /// <list type="bullet">
        /// <item>NO: Normal market (عادی) => ISIN end by 1</item>
        /// <item>OL: Odd lot market (خرده فروشی) => ISIN end by 2</item>
        /// <item>BY: Buying-in market (جبرانی) => ISIN end by 3</item>
        /// <item>BK: Block market (بلوکی) => ISIN end by 4</item>
        /// <item>ID: Index market (شاخص)</item>
        /// <item>UI: Unlisted instruments</item>
        /// </list>
        /// </summary>
        public string MarketSegment { get; set; }
        /// <summary>
        /// کد گروه نماد (CGrValCot)
        /// </summary>
        public string InstrumentGroupId { get; set; }
        /// <summary>
        /// کد تابلو (CComVal)
        /// </summary>
        public string BoardCode { get; set; }
        /// <summary>
        /// کد گروه صنعت (CSecVal)
        /// </summary>
        public string SectorCode { get; set; }
        /// <summary>
        /// کد زير گروه صنعت (CSoSecVal)
        /// </summary>
        public string SubSectorCode { get; set; }
        /// <summary>
        /// ميزان تاخير تسويه (YDeComp)
        /// </summary>
        public int SettlementDelayType { get; set; }
        /// <summary>
        /// تعداد سهام - سرمايه (ZTitad)
        /// </summary>
        public decimal SharesOrBondsNumber { get; set; }
        /// <summary>
        /// حداقل حجم مجاز قابل سفارش (QtitMinSaiOmProd)
        /// </summary>
        public long MinimumEnterableQuantity { get; set; }
        /// <summary>
        /// حداکثر حجم مجاز قابل سفارش (QtitMaxSaiOmProd)
        /// </summary>
        public long MaximumnterableQuantity { get; set; }
        /// <summary>
        /// قيمت اسمی (QNmVlo)
        /// </summary>
        public decimal ParValue { get; set; }
        /// <summary>
        /// تيک - کوچک ترين واحد قيمتي قابل معامله - قيمت فقط ضريبي از اين مقدار مي تواند باشد. (QPasCotFxeVal)
        /// </summary>
        public decimal FixedPriceTick { get; set; }
        /// <summary>
        /// کوچک ترين تعداد سهام قابل معامله - تعداد سهام فقط ضريبي از اين مقدار مي تواند باشد. (QQtTranMarVal)
        /// </summary>
        public long LotSize { get; set; }
        /// <summary>
        /// حجم مبنا (BaseVol)
        /// </summary>
        public long BaseVolume { get; set; }
        /// <summary>
        /// تاريخ اولين روز معاملاتي نماد (DInMar)
        /// </summary>
        public DateTime FirstTradingDate { get; set; }
        /// <summary>
        /// واحد قيمت - 1:قيمت - 2:درصد (YUniExpP)
        /// </summary>
        public byte UnitPriceType { get; set; }
        /// <summary>
        /// حداکثر قيمت مجاز (PSaiSMaxOkValMdv)
        /// </summary>
        public decimal HighIntermediateThreshold { get; set; }
        /// <summary>
        /// حداقل قيمت مجاز (PSaiSMinOkValMdv)
        /// </summary>
        public decimal LowIntermediateThreshold { get; set; }
        /// <summary>
        /// نماد فعال است (Valid)
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// نوع تغییر امروز نماد (YOPSJ)
        /// </summary>
        public string EventType { get; set; }
        /// <summary>
        /// تاریخ (DEven)
        /// </summary>
        public DateTime EventDateTime { get; set; }
        /// <summary>
        /// Tse کد یونیک نماد در  (InsCode)
        /// </summary>
        public long InsCode { get; set; }
    }
}
