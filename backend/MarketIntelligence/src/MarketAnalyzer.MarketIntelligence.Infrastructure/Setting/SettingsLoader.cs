using MarketAnalyzer.MarketIntelligence.Domain;
using System.Reflection;
using System.Text.Json;

namespace MarketAnalyzer.Infrastructure
{
    public static class SettingsLoader
    {
        public static UrlSettings LoadUrlSettings()
        {
            return LoadSettings<UrlSettings>("url_settings.json");
        }

        //public static DbMarketSetting LoadDbMarketSettings()
        //{
        //    return LoadSettings<DbMarketSetting>("dbmarket_settings.json");
        //}

        private static T LoadSettings<T>(string fileName) where T : new()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var folder = Path.GetDirectoryName(assembly.Location);
            var filePath = Path.Combine(folder, "Setting", fileName);

            if (!File.Exists(filePath))
            {
                // اگر فایل نبود، یک نمونه‌ی خالی برگردون
                return new T();
            }

            try
            {
                var json = File.ReadAllText(filePath);
                var item = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return item ?? new T();
            }
            catch (Exception ex) 
            {
                throw ex;
                // اگر JSON خراب بود، نمونه‌ی خالی برگردون
                //return new T();
            }
        }
    }
}
