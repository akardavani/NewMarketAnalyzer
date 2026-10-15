using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.Sejam.Models
{
    public class SejamFinancialInfo
    {
        /// <summary>
        /// ارزش دارایی
        /// </summary>
        public long? AssetsValue { get; set; }
        /// <summary>
        /// متوسط درآمد ماهیانه
        /// </summary>
        public long? InComingAverage { get; set; }
        /// <summary>
        /// مبلغ معاملات بورسهای اوراق بهادار و فرابورس
        /// </summary>
        public long? SExchangeTransaction { get; set; }
        /// <summary>
        /// مبلغ معاملات بورسهای کالایی
        /// </summary>
        public long? CExchangeTransaction { get; set; }
        /// <summary>
        /// مبلغ معاملات بورسهای خارج از کشور
        /// </summary>
        public long? OutExchangeTransaction { get; set; }
        /// <summary>
        /// ارزش ریالی معاملات
        /// </summary>

        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public SejamTransactionLevel? TransactionLevel { get; set; }
        /// <summary>
        /// کد مشخص کننده میزان آشنایی با مفاهیم مالی بورس
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public SejamTradingKnowledgeLevel? TradingKnowledgeLevel { get; set; }
        /// <summary>
        /// منبع تأمین وجه سرمایه گذاری و هدف مشتری از
        /// انجام معاملات در بازار سرمایه ایران یا بورس کالاهای ایرانی
        /// </summary>
        public string CompanyPurpose { get; set; }
        /// <summary>
        /// نام مرجع رتبه بندی کننده
        /// </summary>
        public string ReferenceRateCompany { get; set; }
        /// <summary>
        /// تاریخ رتبه بندی
        /// </summary>
        public DateTime? RateDate { get; set; }
        /// <summary>
        /// رتبه اخذ شده
        /// </summary>
        public long? Rate { get; set; }
        /// <summary>
        /// لیستی از شرکتهای کارگزاری که شخص از طریق
        /// آنها اقدام به معامله کرده است یا منبع تأمین وجه
        /// شخص حقوقی
        /// </summary>
        public List<SejamBrokerEntry> FinancialBrokers { get; set; }
    }
}