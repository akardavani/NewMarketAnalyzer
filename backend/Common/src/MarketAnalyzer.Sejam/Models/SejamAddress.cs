namespace MarketAnalyzer.Sejam.Models
{
    public class SejamAddress
    {
        /// <summary>
        /// کد پستی
        /// </summary>
        public string PostalCode { get; set; }
        /// <summary>
        /// کشور
        /// </summary>
        public SejamRegion Country { get; set; }
        /// <summary>
        /// استان
        /// </summary>
        public SejamRegion Province { get; set; }
        /// <summary>
        /// شهر
        /// </summary>
        public SejamRegion City { get; set; }
        /// <summary>
        /// بخش
        /// </summary>
        public SejamRegion Section { get; set; }
        /// <summary>
        /// پیش شماره شهر
        /// </summary>
        public string CityPrefix { get; set; }
        /// <summary>
        /// خیابان
        /// </summary>
        public string RemnantAddress { get; set; }
        /// <summary>
        /// کوچه
        /// </summary>
        public string Alley { get; set; }
        /// <summary>
        /// پلاک
        /// </summary>
        public string Plaque { get; set; }
        /// <summary>
        /// تلفن ثابت
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// پیش شماره کشور
        /// </summary>
        public string CountryPrefix { get; set; }
        /// <summary>
        /// شماره تلفن همراه تماس اضطراری
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// شماره تلفن ثابت تماس اضطراری
        /// </summary>
        public string EmergencyTel { get; set; }
        /// <summary>
        /// پیش شماره تلفن ثابت اضطراری
        /// </summary>
        public string EmergencyTelCityPrefix { get; set; }
        /// <summary>
        /// پیش شماره فکس
        /// </summary>
        public string FaxPrefix { get; set; }
        /// <summary>
        /// شماره فکس
        /// </summary>
        public string Fax { get; set; }
        /// <summary>
        /// پست الکترونیکی
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// آدرس وب سایت
        /// </summary>
        public string Website { get; set; }
    }
}