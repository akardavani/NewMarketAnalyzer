namespace MarketAnalyzer.DataProvider.ImeModels
{
    /// <summary>
    /// اطلاعات کالای بازار فیزیکی
    /// </summary>
    public class Commodity
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public int ParentId { get; set; }
        public string Description { get; set; }
        public string PersianName { get; set; }
    }
}
