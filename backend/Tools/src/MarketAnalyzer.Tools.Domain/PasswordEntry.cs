using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Tools.Domain
{
    /// <summary>یک ورودی رمز؛ فقط blob رمزنگاری‌شده و IV ذخیره می‌شود.</summary>
    public class PasswordEntry : AggregateRoot<int>
    {
        public PasswordEntry(int id) : base(id) { }
        public int UserId { get; private set; }
        public string Cipher { get; private set; } = string.Empty; // base64
        public string Iv { get; private set; } = string.Empty;     // base64
        public static PasswordEntry Create(int userId) => new(0) { UserId = userId };
        public void Update(string cipher, string iv) { Cipher = cipher; Iv = iv; }
    }
}
