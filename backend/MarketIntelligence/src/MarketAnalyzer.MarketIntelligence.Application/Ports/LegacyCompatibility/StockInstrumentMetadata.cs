namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public sealed class StockInstrumentMetadata
    {
        /// <summary>
        /// Instrument mnemonic code.
        /// </summary>
        public string InstrumentMnemonic { get; set; }
        /// <summary>
        /// 18-character instrument name
        /// نام نماد
        /// </summary>
        public string EnglishName { get; set; }
        /// <summary>
        /// Code for issuing company.
        /// </summary>
        public string FourDigitCompanyCode { get; set; }
        /// <summary>
        /// 30-character AFC name for issuing company.
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 30-character instrument name.
        /// اسم نماد بصورت 30 کاراکتری
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ISIN foreign cash product.
        /// شناسه نماد؛ مثلاً برای فولاد: IRO1FOLD0009
        /// </summary>
        public string CIsin { get; set; }        
        /// <summary>
        /// Instrument group ID.
        /// </summary>
        public string InstrumentGroupId { get; set; }
        /// <summary>
        /// Market Segment
        /// </summary>
        public string MarketSegment { get; set; }
        public long BoardCode { get; set; }
        public long InstrumentType { get; set; }
        public long SharesOrBondsNumber { get; set; }

        public string SectorCode { get; set; }
        public string SubSectorCode { get; set; }
        /// <summary>
        /// حجم مبنا
        /// </summary>
        public long BaseVol { get; set; }
        /// <summary>
        /// Type of instrument.
        /// معرف نمادهای هم گروه
        /// </summary>
        public long? YVal { get; set; }
        /// <summary>
        /// احتمالاً نوع بازار را برگشت می دهد.
        /// </summary>
        public long Flow { get; set; }
        public int Valid { get; set; }
    }
}


