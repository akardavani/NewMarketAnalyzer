namespace System
{
    public static class DecimalExtensions
    {
        /// <summary>
        /// اگر مثبت بود برگردون - وگرنه صفر
        /// </summary>
        public static decimal PositivePart(this decimal value)
        {
            return value > 0 ? value : 0;
        }

        /// <summary>
        /// اگر منفی بود برگردون - وگرنه صفر
        /// </summary>
        public static decimal NegativePart(this decimal value)
        {
            return value < 0 ? -value : 0;
        }
    }
}
