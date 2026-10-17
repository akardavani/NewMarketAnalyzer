namespace MarketAnalyzer.UI.Web
{
    public class UiLink
    {
        public string Url { get; set; } = string.Empty;
        public string DisplayText { get; set; } = string.Empty;
        //public string? ImageUrl { get; set; } // در صورت تمایل به نمایش عکس
        public string? Icon { get; set; } // در صورت تمایل به نمایش عکس
        public string? Target { get; set; } = "_blank";
        public string? CssClass { get; set; }
    }
}
