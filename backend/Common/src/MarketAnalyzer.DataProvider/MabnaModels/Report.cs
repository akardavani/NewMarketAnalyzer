using System;

namespace MarketAnalyzer.DataProvider.MabnaModels
{
    public class Report : EntityBase
    {
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public DateTime? FiscalYear { get; set; }
        public Stock Stock { get; set; }
        public ReportTitle Title { get; set; }

        public string TitleId => Title?.Id;
        public string CompanyId => Stock?.Company?.Id;
    }
}
