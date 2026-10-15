using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace System
{
    public static class ServiceProviderExtensions
    {
        public static async Task<ICollection<T>> ReadJsonDataAsync<T>(this IServiceProvider serviceProvider, string fileName, string folderName = default)
        {
            var environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
            var filePath = Path.Combine(environment.ContentRootPath, "Data", $"{fileName}.json");
            if (!string.IsNullOrEmpty(folderName))
            {
                filePath = Path.Combine(environment.ContentRootPath, $"Data\\{folderName}", $"{fileName}.json");
            }
            
            if (File.Exists(filePath))
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return await JsonSerializer.DeserializeAsync<ICollection<T>>(stream, new JsonSerializerOptions()
                {
                    ReadCommentHandling = JsonCommentHandling.Skip
                });
            }

            return null;
        }

        public static async Task SaveJsonAsync<TData>(this IServiceProvider serviceProvider, string fileName, TData data)
        {
            var environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
            var filePath = Path.Combine(environment.ContentRootPath, "Data", $"{fileName}.json");
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                //ReferenceHandler = ReferenceHandler.Preserve
            };

            var directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(stream, data, options);
        }
    }
}
