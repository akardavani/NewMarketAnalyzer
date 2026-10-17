//namespace MarketIntelligence.Infrastructure
//{
//    public static class StringExtensions
//    {
//        public static string ThousandsSeparator(this decimal value)
//        {
//            return value.ToString("#,##0");
//        }

//        public static string ThousandsSeparatorWithDecimals(this decimal value)
//        {
//            return value.ToString("#,##0.00");
//        }

//        public static string ThousandsSeparator(this long value)
//        {
//            return value.ToString("#,##0");
//        }

//        public static string NormalizeArabicName(this string name)
//        {
//            if (string.IsNullOrEmpty(name))
//                return name;

//            return name.Replace("ي", "ی").Replace("ك", "ک").Trim();
//        }
//    }
//}
