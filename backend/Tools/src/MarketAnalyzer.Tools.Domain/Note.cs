using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Tools.Domain
{
    /// <summary>
    /// یادداشت شخصی کاربر
    /// </summary>
    public class Note : AggregateRoot<int>
    {
        public Note(int id) : base(id)
        {
        }

        public int UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        /// <summary>HTML پاک‌سازی‌شده</summary>
        public string Content { get; private set; } = string.Empty;
        /// <summary>رنگ یادداشت (hex مثل #ffd54f)</summary>
        public string? Color { get; private set; }

        public static Note Create(int userId, string title, string content, string? color = null)
        {
            return new Note(0) { UserId = userId, Title = title ?? string.Empty, Content = content ?? string.Empty, Color = color };
        }

        public void Update(string title, string content, string? color)
        {
            Color = color;
            Title = title ?? string.Empty;
            Content = content ?? string.Empty;
        }
    }
}
