namespace MarketAnalyzer.Abstractions
{
    public static class JwtTokenPayloads
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public const string UserIdClaim = "uid";

        /// <summary>
        /// نام کامل کاربر
        /// </summary>
        public const string FullNameClaim = "fn";

        /// <summary>
        /// نام کاربر
        /// </summary>
        public const string NameClaim = "name";

        /// <summary>
        /// نقش کاربر
        /// </summary>
        public const string RoleClaim = "role";

        /// <summary>
        /// شناسه برنامه کاربردی
        /// </summary>
        public const string ApplicationClaim = "app";

        /// <summary>
        /// کد ملی
        /// </summary>
        public const string NationalCodeClaim = "nc";

        /// <summary>
        /// مجوز صدور و ابطال اینترنتی
        /// </summary>
        public const string RequestPermitClaim = "rp";
    }
}
