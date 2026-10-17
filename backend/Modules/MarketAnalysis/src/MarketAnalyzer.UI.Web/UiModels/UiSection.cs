namespace MarketAnalyzer.UI.Web
{
    public class UiSection
    {
        public string Title { get; set; } = "";
        public UiSectionType Type { get; set; }
        public object Data { get; set; } = default!;
    }

}
