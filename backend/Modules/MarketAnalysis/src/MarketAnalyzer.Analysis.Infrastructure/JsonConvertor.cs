using System.Text.Encodings.Web;
using System.Text.Json;

namespace MarketAnalyzer.Analysis.Infrastructure
{
    public static class JsonConvertor
    {
        public static T ReadJsonData<T>(string fileName)
        {
            var currentDirectory = Environment.CurrentDirectory;

            for (int i = 0; i < 2; i++)
            {
                currentDirectory = Directory.GetParent(currentDirectory).FullName;
            }
            //var path = $@"{currentDirectory}\Services\OmsRequest\";

            var filePath = Path.Combine(currentDirectory, $"{fileName}.json");
            if (File.Exists(filePath))
            {
                var content = File.ReadAllText(filePath);

                var ss = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions()
                {
                    ReadCommentHandling = JsonCommentHandling.Skip
                });

                return ss;
            }

            return default(T);
        }

        public static ICollection<T> ReadJsonDataCollection<T>(string fileName, string path)
        {
            var filePath = Path.Combine(path, $"{fileName}.json");
            if (File.Exists(filePath))
            {
                var content = File.ReadAllText(filePath);

                return JsonSerializer.Deserialize<ICollection<T>>(content, new JsonSerializerOptions()
                {
                    ReadCommentHandling = JsonCommentHandling.Skip
                });
            }

            return null;
        }

        public static void WriteJsonDataList<T>(List<T> data, string fileName, string path)
        {
            var filePath = Path.Combine(path, $"{fileName}.json");

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            File.WriteAllText(filePath, json);
        }

        public static ICollection<T> ReadJsonByString<T>(string content)
        {
            return JsonSerializer.Deserialize<ICollection<T>>(content, new JsonSerializerOptions()
            {
                ReadCommentHandling = JsonCommentHandling.Skip
            });
        }

        public static ICollection<T> ReadJsonByString00<T>(string filePath)
        {
            var content = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<ICollection<T>>(content, new JsonSerializerOptions()
            {
                ReadCommentHandling = JsonCommentHandling.Skip
            });

        }
    }
}
