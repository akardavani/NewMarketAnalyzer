namespace MarketAnalyzer.Domain
{
    public class SignalModel
    {
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public DateTime UtcDateTime { get; set; }
        public decimal SignalPrice { get; set; }
        public decimal StopLoss { get; set; }
        public decimal StopLoss2 { get; set; }
        public decimal StopLossPercent { get; set; }
        public decimal StopLossPercent2 { get; set; }
        public decimal TakeProfit_1 { get; set; }
        public decimal TakeProfit_2 { get; set; }
        public decimal TakeProfit_3 { get; set; }
        //public PositionEnum TriggerPosition { get; set; }
        //public OpenClosePositionEnum TriggerOpenClosePosition { get; set; }
    }
}
