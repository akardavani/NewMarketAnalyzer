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

        public static Note Create(int userId, string title, string content)
        {
            return new Note(0) { UserId = userId, Title = title ?? string.Empty, Content = content ?? string.Empty };
        }

        public void Update(string title, string content)
        {
            Title = title ?? string.Empty;
            Content = content ?? string.Empty;
        }
    }
}
