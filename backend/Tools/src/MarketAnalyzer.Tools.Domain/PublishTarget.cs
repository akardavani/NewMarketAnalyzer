using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Tools.Domain
{
    /// <summary>پوشه قابل انتشار (فقط ابزار توسعه)</summary>
    public class PublishTarget : AggregateRoot<int>
    {
        public PublishTarget(int id) : base(id) { }

        public string Key { get; private set; } = string.Empty;
        public string Title { get; private set; } = string.Empty;
        public string? Group { get; private set; }
        public string Path { get; private set; } = string.Empty;
        public string OutputPath { get; private set; } = string.Empty;
        public int DisplayOrder { get; private set; }
        public bool IsActive { get; private set; } = true;

        public static PublishTarget Create() => new(0);

        public void Update(string key, string title, string? group, string path, string outputPath, int displayOrder, bool isActive)
        {
            Key = key; Title = title; Group = group; Path = path;
            OutputPath = outputPath; DisplayOrder = displayOrder; IsActive = isActive;
        }
    }
}
