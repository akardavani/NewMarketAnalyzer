using System.Text;
using System.Text.Json;


namespace MarketAnalyzer.Tools.Domain
{
    public static class JsonConvertor
    {
        public static ICollection<T> ReadJsonData<T>(string fileName)
        {
            var filePath = GetFilePath(fileName);

            if (!System.IO.File.Exists(filePath))
                return new List<T>();

            var content = System.IO.File.ReadAllText(
                filePath,
                Encoding.UTF8);

            // خطای JSON خراب عمداً پنهان نمی‌شود؛
            // سابقهٔ خراب نباید به‌عنوان سابقهٔ خالی تلقی شود.
            return JsonSerializer.Deserialize<List<T>>(
                content,
                new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    PropertyNameCaseInsensitive = true
                }) ?? new List<T>();
        }

        public static void WriteJsonData<T>(
            List<T> data,
            string fileName)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var filePath = GetFilePath(fileName);
            var directory = Path.GetDirectoryName(filePath)!;

            Directory.CreateDirectory(directory);

            var json = JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            // ابتدا در فایل موقت نوشته می‌شود تا خطای نوشتن
            // محتوای فایل سابقهٔ فعلی را ناقص نکند.
            var temporaryPath =
                filePath + "." + Guid.NewGuid().ToString("N") + ".tmp";

            try
            {
                System.IO.File.WriteAllText(
                    temporaryPath,
                    json,
                    new UTF8Encoding(false));

                System.IO.File.Move(
                    temporaryPath,
                    filePath,
                    overwrite: true);
            }
            finally
            {
                if (System.IO.File.Exists(temporaryPath))
                    System.IO.File.Delete(temporaryPath);
            }
        }

        private static string GetFilePath(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) ||
                fileName == "." ||
                fileName == ".." ||
                fileName.Contains("/") ||
                fileName.Contains("\\") ||
                fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ArgumentException(
                    "نام فایل سابقه نامعتبر است.",
                    nameof(fileName));
            }

            return Path.Combine(
                Environment.CurrentDirectory,
                "Data",
                $"{fileName}.json");
        }
    }
}
