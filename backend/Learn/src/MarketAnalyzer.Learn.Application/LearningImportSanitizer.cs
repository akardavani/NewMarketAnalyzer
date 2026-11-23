using Ganss.Xss;

namespace MarketAnalyzer.Learn.Application;

public sealed class LearningImportSanitizer : ILearningImportSanitizer
{
    private static readonly HtmlSanitizer Sanitizer = CreateSanitizer();

    public string Sanitize(string originalHtml) =>
        string.IsNullOrWhiteSpace(originalHtml) ? string.Empty : Sanitizer.Sanitize(originalHtml);

    private static HtmlSanitizer CreateSanitizer()
    {
        var sanitizer = new HtmlSanitizer();
        foreach (var tag in new[] { "pre", "code", "table", "thead", "tbody", "tr", "th", "td", "blockquote", "hr", "del", "ins", "sub", "sup" })
            sanitizer.AllowedTags.Add(tag);
        sanitizer.AllowedAttributes.Add("class");
        sanitizer.AllowedAttributes.Add("dir");
        return sanitizer;
    }
}
