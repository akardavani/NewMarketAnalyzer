using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Tools.Domain
{
    /// <summary>تنظیمات کاسه‌ی رمز هر کاربر. سرور هرگز master password یا کلید را نمی‌بیند.</summary>
    public class PasswordVault : AggregateRoot<int>
    {
        public PasswordVault(int id) : base(id) { }
        public int UserId { get; private set; }
        public string KdfSalt { get; private set; } = string.Empty;      // base64
        public int KdfIterations { get; private set; }
        public string VerifierCipher { get; private set; } = string.Empty; // base64 (AES-GCM of known plaintext)
        public string VerifierIv { get; private set; } = string.Empty;     // base64
        public static PasswordVault Create(int userId) => new(0) { UserId = userId };
        public void SetKeys(string salt, int iterations, string verifierCipher, string verifierIv)
        { KdfSalt = salt; KdfIterations = iterations; VerifierCipher = verifierCipher; VerifierIv = verifierIv; }
    }
}
