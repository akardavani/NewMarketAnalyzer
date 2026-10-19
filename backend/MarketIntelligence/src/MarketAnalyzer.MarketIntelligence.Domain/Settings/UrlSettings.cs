namespace MarketAnalyzer.MarketIntelligence.Domain
{
    public class UrlSettings
    {
        /// <summary>
        /// دیتای سایت TSE
        /// </summary>
        public TseTmcSettings TseTmcSettings { get; set; }
        /// <summary>
        /// دیتای وب سرویس TSE
        /// </summary>
        public TseTmcWebServiceSetting TseTmcWebServiceSettings { get; set; }
        /// <summary>
        /// دیتای طلا و دلار و ...
        /// </summary>
        public CandleSettings tgjuSettings { get; set; }
        public CandleSettings WallexSettings { get; set; }
        public CandleSettings NobitexSettings { get; set; }
        /// <summary>
        /// دیتای کالا - فیوچرز و ...
        /// </summary>
        public CandleSettings ImeSettings { get; set; }
        /// <summary>
        /// دیتای کالا - فیوچرز و ...
        /// </summary>
        public CandleSettings BaseImeSettings { get; set; }
        /// <summary>
        /// دیتای خانه
        /// </summary>
        public CandleSettings KilidSettings { get; set; }
        /// <summary>
        /// دیتای سهام
        /// </summary>
        public CandleSettings NahayatnegarSettings { get; set; }
        /// <summary>
        /// دیتای کریپتو
        /// </summary>
        public CandleSettings KucoinSettings { get; set; }
        /// <summary>
        /// دیتای همه چیز ...
        /// </summary>
        public CandleSettings InvestingSettings { get; set; }
        /// <summary>
        /// دیتای ماشین
        /// </summary>
        public CandleSettings HamrahMechanicSettings { get; set; }
        /// <summary>
        /// دیتای صندوق های سرمایه گذاری - فیپایران
        /// </summary>
        public InvestmentFundSetting InvestmentFundInfo { get; set; }
    }
}
