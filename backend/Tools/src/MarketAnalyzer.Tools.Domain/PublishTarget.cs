using MarketAnalyzer.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Tools.Domain
{
    /// <summary>یک پوشه‌ی پروژه با مسیر کامل (مثلا D:\...\backend\MarketAnalyzer.Api)</summary>
    public class PublishFolder
    {
        private PublishFolder() { }
        public PublishFolder(string name, string path) { Name = name; Path = path; }
        /// <summary>نام پوشه داخل zip (مثلا api)</summary>
        public string Name { get; private set; } = string.Empty;
        /// <summary>مسیر کامل پوشه‌ی پروژه</summary>
        public string Path { get; private set; } = string.Empty;
    }

    /// <summary>یک مورد انتشار (مثلا stage-web): چند پوشه‌ی پروژه با مسیر کامل → یک zip (فقط ابزار توسعه)</summary>
    public class PublishTarget : AggregateRoot<int>
    {
        private readonly List<PublishFolder> _folders = new();

        public PublishTarget(int id) : base(id) { }

        public string Key { get; private set; } = string.Empty;
        public string Title { get; private set; } = string.Empty;
        public string OutputPath { get; private set; } = string.Empty;
        public int DisplayOrder { get; private set; }
        public bool IsActive { get; private set; } = true;
        public IReadOnlyList<PublishFolder> Folders => _folders;

        public static PublishTarget Create() => new(0);

        public void Update(string key, string title, string outputPath, int displayOrder, bool isActive, IEnumerable<(string Name, string Path)> folders)
        {
            Key = key; Title = title;
            OutputPath = outputPath; DisplayOrder = displayOrder; IsActive = isActive;
            _folders.Clear();
            _folders.AddRange(folders.Select(f => new PublishFolder(f.Name, f.Path)));
        }
    }
}
