using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Tools.Domain
{
    public enum BookmarkKind { Link = 0, Pdf = 1, Image = 2, Video = 3, Document = 4 }

    /// <summary>پوشهٔ تودرتو برای لینک‌ها</summary>
    public class BookmarkCollection : AggregateRoot<int>
    {
        public BookmarkCollection(int id) : base(id) { }
        public int UserId { get; private set; }
        public int? ParentId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public static BookmarkCollection Create(int userId, string name, int? parentId) => new(0) { UserId = userId, Name = name, ParentId = parentId };
        public void Update(string name, int? parentId) { Name = name; ParentId = parentId; }
    }

    public class BookmarkTag : AggregateRoot<int>
    {
        public BookmarkTag(int id) : base(id) { }
        public int UserId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public static BookmarkTag Create(int userId, string name) => new(0) { UserId = userId, Name = name };
        public void Rename(string name) => Name = name;
    }

    public class Bookmark : AggregateRoot<int>
    {
        public Bookmark(int id) : base(id) { }
        public int UserId { get; private set; }
        public int? CollectionId { get; private set; }
        public string Url { get; private set; } = string.Empty;
        public string Title { get; private set; } = string.Empty;
        public string Note { get; private set; } = string.Empty;
        public string? ImageUrl { get; private set; }
        public BookmarkKind Kind { get; private set; }
        public bool IsFavorite { get; private set; }
        public List<BookmarkTag> Tags { get; private set; } = new();

        public static Bookmark Create(int userId) => new(0) { UserId = userId };
        public void Update(string url, string title, string note, string? imageUrl, BookmarkKind kind, int? collectionId, IEnumerable<BookmarkTag> tags)
        {
            Url = url; Title = title; Note = note; ImageUrl = imageUrl; Kind = kind; CollectionId = collectionId;
            Tags.Clear(); Tags.AddRange(tags);
        }
        public void SetFavorite(bool v) => IsFavorite = v;
    }
}
