namespace MarketAnalyzer.DataProvider.ImeModels
{
    public class WarehouseApiResponse<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public int TotalCount { get; set; }
        public T Data { get; set; }
    }
}
