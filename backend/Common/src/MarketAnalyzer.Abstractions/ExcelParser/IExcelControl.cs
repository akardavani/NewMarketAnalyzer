namespace MarketAnalyzer.Abstractions
{
    public interface IExcelControl
    {
        string Name { get; set; }
        string AlternativeText { get; set; }
        string LinkedCell { get; set; }

        void AdjustPositionAndSize();
        void SetSize(int width, int height);
        void SetPosition(int pixelTop, int pixelLeft);
        void SetPosition(int row, int rowOffsetPixels, int column, int columnOffsetPixels);
    }
}
