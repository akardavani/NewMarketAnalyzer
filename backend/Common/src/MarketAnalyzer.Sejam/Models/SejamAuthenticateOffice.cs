namespace MarketAnalyzer.Sejam.Models
{
    public class SejamAuthenticateOffice
    {
        /// <summary>
        /// شناسه استان
        /// </summary>
        public long ProvinceId { get; set; }
        /// <summary>
        /// نام استان
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// شناسه شهر
        /// </summary>
        public long CityId { get; set; }
        /// <summary>
        /// نام شهر
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// شماره تلفن
        /// </summary>
        public string Telephone { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// نام مسئول
        /// </summary>
        public string ResponsibleName { get; set; }
        /// <summary>
        /// نوع مرکز احراز هویت
        /// </summary>
        public SejamAuthenticateOfficeType Type { get; set; }
    }
}