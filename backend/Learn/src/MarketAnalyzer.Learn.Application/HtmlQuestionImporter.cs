using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MarketAnalyzer.Learn.Application;

/// <summary>Importer for the current numbered h2 question HTML format.</summary>
public sealed class HtmlQuestionImporter : ILearningContentImporter
{
    private static readonly Regex QuestionBlock = new(
        @"<h2\b[^>]*>(?<heading>.*?)</h2>(?<body>.*?)(?=<h2\b|\z)",
        RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
    private static readonly Regex Tags = new("<[^>]+>", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex Number = new(@"^\s*(?<number>[0-9۰-۹٠-٩]+)\s*[.．、:-]?\s*(?<title>.*)$", RegexOptions.Compiled | RegexOptions.Singleline);

    public string Key => "html-numbered-questions";

    public bool CanImport(string fileName, string content) =>
        fileName.EndsWith(".html", StringComparison.OrdinalIgnoreCase) &&
        content.Contains("<h2", StringComparison.OrdinalIgnoreCase);

    public Task<IReadOnlyList<ImportPreviewItem>> ParseAsync(string fileName, string content, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = new List<ImportPreviewItem>();
        foreach (Match match in QuestionBlock.Matches(content))
        {
            var headingHtml = match.Groups["heading"].Value;
            var headingText = NormalizeText(headingHtml);
            var parsedHeading = Number.Match(headingText);
            // These source files also use h2 for unnumbered summaries. They are
            // not question records and must not be imported as standalone items.
            if (!parsedHeading.Success) continue;

            var externalNumber = NormalizeDigits(parsedHeading.Groups["number"].Value);
            var title = parsedHeading.Groups["title"].Value.Trim();
            if (string.IsNullOrWhiteSpace(title)) title = $"سؤال {externalNumber}";

            var original = match.Value;
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(original)));
            result.Add(new ImportPreviewItem(externalNumber, title, original, hash));
        }
        return Task.FromResult<IReadOnlyList<ImportPreviewItem>>(result);
    }

    private static string NormalizeText(string html) => WebUtility.HtmlDecode(Tags.Replace(html, " "))
        .Replace('\u00a0', ' ').Trim();

    private static string NormalizeDigits(string value)
    {
        var result = new StringBuilder(value.Length);
        foreach (var character in value)
            result.Append(character switch
            {
                >= '۰' and <= '۹' => (char)('0' + character - '۰'),
                >= '٠' and <= '٩' => (char)('0' + character - '٠'),
                _ => character
            });
        return result.ToString();
    }
}
