namespace MarketIntelligence.Domain
{
    public class TseTmcSettings
    {
        public string TseTmcLoaderUrl { get; set; }
        public string TseTmcInfo { get; set; }
        public string InstrumentList { get; set; }
        public string TseTmcETF { get; set; }
        public string TseTmcIndexSymbol { get; set; } //  ایندکس ها
        public string TseTmcIndexCandle { get; set; } // کندل ایندکس ها
        public string MofidEasyFilterData { get; set; }
    }
}
