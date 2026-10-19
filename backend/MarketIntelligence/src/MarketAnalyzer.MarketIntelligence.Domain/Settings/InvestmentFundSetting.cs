namespace MarketAnalyzer.MarketIntelligence.Domain
{
    public class InvestmentFundSetting
    {
        /// <summary>
        /// مشخصات صندوق 
        /// </summary>
        public string FundsUrl { get; set; }
        /// <summary>
        /// نوع صندوق
        /// </summary>
        public string FundTypesUrl { get; set; }
        /// <summary>
        /// اطلاعات کلی صندوق
        /// </summary>
        public string FundInfoUrl { get; set; }
        /// <summary>
        /// قیمت صدور و ابطال صندوق
        /// </summary>
        public string FundChartUrl { get; set; }
        /// <summary>
        /// خالص ارزش دارایی
        /// </summary>
        public string FundNetassetChartUrl { get; set; }
        /// <summary>
        /// آلفا بتا صندوق
        /// </summary>
        public string FundAlphaBetaChartUrl { get; set; }
        /// <summary>
        /// سود زیان روزانه هفتگی و ماهانه و سالانه صندوق
        /// </summary>
        public string FundEfficiencyUrl { get; set; }        
        /// <summary>
        /// ترکیب دارایی‌ها 
        /// </summary>
        public string FundPortfolioChartUrl { get; set; }    
                
    }
}
