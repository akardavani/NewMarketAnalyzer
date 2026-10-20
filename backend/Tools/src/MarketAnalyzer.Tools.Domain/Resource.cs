using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Tools.Domain
{
    /// <summary>منبع/یادداشت دسته‌بندی‌شده با تگ، لینک و فایل</summary>
    public class Resource : AggregateRoot<int>
    {
        public Resource(int id) : base(id) { }

        public int UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string? Link { get; private set; }
        public List<ResourceTag> Tags { get; private set; } = new();
        public List<ResourceFile> Files { get; private set; } = new();

        public static Resource Create(int userId) => new(0) { UserId = userId };

        public void Update(string title, string description, string? link, IEnumerable<ResourceTag> tags)
        {
            Title = title; Description = description; Link = link;
            Tags.Clear();
            Tags.AddRange(tags);
        }
    }

    public class ResourceTag : AggregateRoot<int>
    {
        public ResourceTag(int id) : base(id) { }
        public int UserId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public static ResourceTag Create(int userId, string name) => new(0) { UserId = userId, Name = name };
    }

    public class ResourceFile : AggregateRoot<int>
    {
        public ResourceFile(int id) : base(id) { }
        public int ResourceId { get; private set; }
        public string FileName { get; private set; } = string.Empty;
        public string ContentType { get; private set; } = "application/octet-stream";
        public long Size { get; private set; }
        public byte[] Content { get; private set; } = Array.Empty<byte>();

        public static ResourceFile Create(int resourceId, string name, string type, byte[] content)
            => new(0) { ResourceId = resourceId, FileName = name, ContentType = type, Size = content.LongLength, Content = content };

        /// <summary>جایگزینی محتوا؛ همان رکورد/شناسه می‌ماند</summary>
        public void Replace(string name, string type, byte[] content)
        { FileName = name; ContentType = type; Size = content.LongLength; Content = content; }
    }
}
