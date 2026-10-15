namespace MarketAnalyzer.Infrastructure.Authentication
{
    public static class AuthenticationConstants
    {
        /// <summary>
        /// The default authorization policy name.
        /// </summary>
        public const string DefaultPolicy = "fund_auth_policy";

        /// <summary>
        /// The name to use in api key authorization policy.
        /// </summary>
        public const string RequiredApiKeyPolicy = "required_api_key_policy";
    }
}
