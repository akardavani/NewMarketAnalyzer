using System.Text.Json.Serialization;

namespace MarketAnalyzer.MarketIntelligence.Infrastructure
{
    public sealed class InvestmentFundMetadata
    {
        /// <summary>
        /// تاریخ آغاز فعالیت صندوق (initiationDate)
        /// nullable چون ممکنه بعضی رکوردها مقدار نداشته باشند
        /// </summary>
        [JsonPropertyName("initiationDate")]
        public DateTime? InitiationDate { get; set; }

        /// <summary>
        /// نوع صندوق (fundType)
        /// nullable برای ایمنی بیشتر
        /// </summary>
        [JsonPropertyName("fundType")]
        public int? FundType { get; set; }

        /// <summary>
        /// شیوه سرمایه‌گذاری (typeOfInvest)
        /// </summary>
        [JsonPropertyName("typeOfInvest")]
        public string TypeOfInvest { get; set; }

        /// <summary>
        /// دوره تقسیم سود (dividendIntervalPeriod)
        /// </summary>
        //[JsonPropertyName("dividendIntervalPeriod")]
        //public int? DividendIntervalPeriod { get; set; }

        /// <summary>
        /// تعداد واحدهای سرمایه‌گذاری شده (investedUnits)
        /// </summary>
        [JsonPropertyName("investedUnits")]
        public long? InvestedUnits { get; set; }

        /// <summary>
        /// شماره ثبت مدیر نزد سازمان (managerSeoRegisterNo)
        /// </summary>
        [JsonPropertyName("managerSeoRegisterNo")]
        public string ManagerSeoRegisterNo { get; set; }

        /// <summary>
        /// آدرس وب‌سایت‌های صندوق (websiteAddress)
        /// </summary>
        [JsonPropertyName("websiteAddress")]
        public List<string> WebsiteAddress { get; set; }

        /// <summary>
        /// وضعیت تکمیل بودن اطلاعات (isCompleted)
        /// </summary>
        [JsonPropertyName("isCompleted")]
        public bool IsCompleted { get; set; }

        /// <summary>
        /// ناشر صندوق (fundPublisher)
        /// nullable چون ممکنه مقدار نداشته باشد
        /// </summary>
        [JsonPropertyName("fundPublisher")]
        public int? FundPublisher { get; set; }

        /// <summary>
        /// اندازه صندوق (fundSize)
        /// </summary>
        [JsonPropertyName("fundSize")]
        public long? FundSize { get; set; }

        /// <summary>
        /// خالص ارزش دارایی‌ها (netAsset)
        /// </summary>
        [JsonPropertyName("netAsset")]
        public long? NetAsset { get; set; }

        /// <summary>
        /// تاریخ آخرین بروزرسانی (date)
        /// </summary>
        [JsonPropertyName("date")]
        public DateTime? Date { get; set; }
    }
}


