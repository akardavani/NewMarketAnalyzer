namespace MarketAnalyzer.DataProvider.TseModels
{
    public class TseSubSector
    {
        /// <summary>
        /// کد زیر گروه (CSoSecVal)
        /// </summary>
        public string SubSectorCode { get; set; }
        /// <summary>
        /// کد گروه صنعت (CSecVal)
        /// </summary>
        public string SectorCode { get; set; }
        /// <summary>
        /// نام زیر گروه صنعت (LSoSecVal)
        /// </summary>
        public string SubSectorName { get; set; }
    }
}
