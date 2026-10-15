namespace System
{
    public static class CommissionHelper
    {
        public static decimal Min(decimal coefficient, decimal value, decimal maxValue)
        {
            var fee = value * coefficient;
            return (maxValue == -1) ? fee : Math.Min(fee, maxValue);
        }
    }
}
