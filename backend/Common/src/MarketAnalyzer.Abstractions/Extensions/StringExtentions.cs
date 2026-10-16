namespace System
{
    public static class StringExtentions
    {
        public static string NormalizeArabicName(this string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            return name.Replace("ي", "ی").Replace("ك", "ک").Trim();
        }

        public static string NormalizePhoneNumber(this string number)
        {
            if (string.IsNullOrEmpty(number))
            {
                return number;
            }

            if (number.StartsWith('+'))
            {
                number = number[1..];
            }

            if (number.StartsWith("98"))
            {
                number = number[2..];
            }

            return (number.Length == 10) ? $"0{number}" : number;
        }

        public static string ThousandsSeparator(this decimal value)
        {
            return value.ToString("#,##0");
        }

        public static string ThousandsSeparatorWithDecimals(this decimal value)
        {
            return value.ToString("#,##0.00");
        }

        public static string ThousandsSeparator(this long value)
        {
            return value.ToString("#,##0");
        }
    }
}
