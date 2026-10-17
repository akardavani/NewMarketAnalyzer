namespace MarketAnalyzer.Abstractions
{
    public class AuthResult
    {
        public string AccessToken { get; set; }
        public int Lifetime { get; set; }
        public bool RequiredTfa { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// شماره موبایل کاربر
        /// <para>در ورود دو مرحله ای مورد نیاز است</para>
        /// </summary>
        public string PhoneNumber { get; set; }
        public string NameIdentifier { get; set; }
    }
}
