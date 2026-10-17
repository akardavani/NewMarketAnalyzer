namespace MarketAnalyzer.Sejam.Requests
{
    internal class TokenRequest
    {
        /// <summary>
        /// نام کاربری
        /// </summary>
        public required string Username { get; init; }
        /// <summary>
        /// کلمه عبور
        /// </summary>
        public required string Password { get; init; }
    }
}