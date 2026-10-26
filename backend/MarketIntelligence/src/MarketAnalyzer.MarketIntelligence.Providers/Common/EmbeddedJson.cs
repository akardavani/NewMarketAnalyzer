using System.Reflection;
using System.Text.Json;

namespace MarketAnalyzer.MarketIntelligence.Providers
{
    /// <summary>خواندن فهرست‌های ثابت که داخل DLL به‌صورت EmbeddedResource آمده‌اند (پوشهٔ Resources).</summary>
    internal static class EmbeddedJson
    {
        private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

        /// <summary>فایل را با نام (مثل crypto-toman.symbols.json) می‌خواند. نبودن فایل خطای برنامه‌نویسی است و استثنا می‌دهد.</summary>
        public static List<T> ReadList<T>(string fileName)
        {
            var assembly = typeof(EmbeddedJson).Assembly;
            var resource = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith("." + fileName, StringComparison.Ordinal))
                           ?? throw new InvalidOperationException($"Embedded resource '{fileName}' was not found in {assembly.GetName().Name}.");

            using var stream = assembly.GetManifestResourceStream(resource)!;
            return JsonSerializer.Deserialize<List<T>>(stream, Options)
                   ?? throw new InvalidOperationException($"Embedded resource '{fileName}' is empty or invalid.");
        }
    }
}
