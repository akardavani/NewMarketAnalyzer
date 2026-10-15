using System;

namespace DataProvider
{
    internal class InstrumentEntry
    {
        public string Id { get; set; }
        public int ExchangeId { get; set; }
        public string CompanyIsin { get; set; }
        public string CompanyCode { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string NormalizedNativeName { get; set; }
        public string Mnemonic { get; set; }
        public int InstrumentTypeCode { get; set; }
        public string MarketSegment { get; set; }
        public string InstrumentGroupId { get; set; }
        public string BoardCode { get; set; }
        public string SectorCode { get; set; }
        public string SubSectorCode { get; set; }
        public int SettlementDelayType { get; set; }
        public long SharesOrBondsNumber { get; set; }
        public long MinimumEnterableQuantity { get; set; }
        public long MaximumEnterableQuantity { get; set; }
        public long ParValue { get; set; }
        public long FixedPriceTick { get; set; }
        public long LotSize { get; set; }
        public long BaseVolume { get; set; }
        public DateTime FirstTradingDate { get; set; }
        public int UnitPriceType { get; set; }
        public long HighIntermediateThreshold { get; set; }
        public long LowIntermediateThreshold { get; set; }
        public string EventType { get; set; }
        public DateTime EventDateTime { get; set; }
        public bool IsActive { get; set; }
        public long InsCode { get; set; }

        //Later Set
        public string RightInstrumentId { get; set; }
        public int InstrumentTypeId { get; set; }
        public string BaseInstrumentId { get; set; }
    }
}
