namespace MarketAnalyzer.DataProvider.TseModels
{
    public enum DataFlow : byte
    {
        None = 0,
        Tse = 1,
        Otc = 2,
        Future = 3,
        OtcBase = 4,
        OtcBaseOther = 5,
        Energy = 6,
        Mercantile = 7
    }
}
