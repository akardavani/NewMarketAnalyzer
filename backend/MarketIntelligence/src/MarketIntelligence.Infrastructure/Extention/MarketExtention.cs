namespace MarketIntelligence.Infrastructure
{
    public static class MarketExtention
    {
        public static string GetOTCBaseMarketPersianColor(string instrumentGroupId)
        {
            return instrumentGroupId switch
            {
                "C1" => "نارنجی",
                "P1" => "زرد",
                "L1" => "قرمز",
                _ => string.Empty,
            };
        }

        public static string SetOTCBaseMarketColor(string instrumentGroupId)
        {
            return instrumentGroupId switch
            {
                "C1" => "orange",
                "P1" => "yellow",
                "L1" => "red",
                _ => string.Empty,
            };
        }
    }
}
