using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace MarketAnalyzer.Domain
{
    public class TseClientTypeResponse
    {
        // پشتیبانی همزمان از System.Text.Json و Newtonsoft.Json
        [JsonProperty("clientType")]
        [JsonPropertyName("clientType")]
        public List<TseClientTypeInfo> ClientType { get; set; } = new();
    }

    public class TseClientTypeInfo : IEntity
    {
        public string Id { get; set; } 

        [JsonProperty("insCode")]
        [JsonPropertyName("insCode")]
        public string InsCode { get; set; } = string.Empty;

        [JsonProperty("recDate")]
        [JsonPropertyName("recDate")]
        public long RecDate { get; set; }

        [JsonProperty("buy_I_Volume")]
        [JsonPropertyName("buy_I_Volume")]
        public double BuyIVolume { get; set; }

        [JsonProperty("buy_N_Volume")]
        [JsonPropertyName("buy_N_Volume")]
        public double BuyNVolume { get; set; }

        [JsonProperty("buy_I_Value")]
        [JsonPropertyName("buy_I_Value")]
        public double BuyIValue { get; set; }

        [JsonProperty("buy_N_Value")]
        [JsonPropertyName("buy_N_Value")]
        public double BuyNValue { get; set; }

        [JsonProperty("buy_N_Count")]
        [JsonPropertyName("buy_N_Count")]
        public int BuyNCount { get; set; }

        [JsonProperty("sell_I_Volume")]
        [JsonPropertyName("sell_I_Volume")]
        public double SellIVolume { get; set; }

        [JsonProperty("buy_I_Count")]
        [JsonPropertyName("buy_I_Count")]
        public double BuyICount { get; set; }

        [JsonProperty("sell_N_Volume")]
        [JsonPropertyName("sell_N_Volume")]
        public double SellNVolume { get; set; }

        [JsonProperty("sell_I_Value")]
        [JsonPropertyName("sell_I_Value")]
        public double SellIValue { get; set; }

        [JsonProperty("sell_N_Value")]
        [JsonPropertyName("sell_N_Value")]
        public double SellNValue { get; set; }

        [JsonProperty("sell_N_Count")]
        [JsonPropertyName("sell_N_Count")]
        public int SellNCount { get; set; }

        [JsonProperty("sell_I_Count")]
        [JsonPropertyName("sell_I_Count")]
        public int SellICount { get; set; }
        
    }
}



//using Newtonsoft.Json;
//using System.Text.Json.Serialization;

//namespace MarketAnalyzer.Domain
//{
//    public class TseClientTypeResponse
//    {
//        // پشتیبانی همزمان از System.Text.Json و Newtonsoft.Json
//        [JsonProperty("clientType")]
//        [JsonPropertyName("clientType")]
//        public List<TseClientTypeInfo> ClientType { get; set; } = new();
//    }

//    public class TseClientTypeInfo
//    {
//        /// <summary>
//        /// کد شناسایی انحصاری نماد در بورس تهران (شناسه ابزار)
//        /// </summary>
//        [JsonProperty("insCode")]
//        public string InsCode { get; set; } = string.Empty;

//        /// <summary>
//        /// تاریخ ثبت رکورد معامله به صورت عدد صحیح (فرمت: yyyyMMdd مانند 20260701)
//        /// </summary>
//        [JsonProperty("recDate")]
//        public long RecDate { get; set; }

//        /// <summary>
//        /// حجم خرید حقیقی (تعداد سهام خریداری شده توسط افراد حقیقی)
//        /// </summary>
//        [JsonProperty("buy_I_Volume")]
//        public double BuyIVolume { get; set; }

//        /// <summary>
//        /// حجم خرید حقوقی (تعداد سهام خریداری شده توسط کدهای حقوقی)
//        /// </summary>
//        [JsonProperty("buy_N_Volume")]
//        public double BuyNVolume { get; set; }

//        /// <summary>
//        /// ارزش خرید حقیقی (ارزش ریالی کل خرید افراد حقیقی)
//        /// </summary>
//        [JsonProperty("buy_I_Value")]
//        public double BuyIValue { get; set; }

//        /// <summary>
//        /// ارزش خرید حقوقی (ارزش ریالی کل خرید کدهای حقوقی)
//        /// </summary>
//        [JsonProperty("buy_N_Value")]
//        public double BuyNValue { get; set; }

//        /// <summary>
//        /// تعداد خریداران حقوقی (تعداد کدهای حقوقی خریدار)
//        /// </summary>
//        [JsonProperty("buy_N_Count")]
//        public int BuyNCount { get; set; }

//        /// <summary>
//        /// حجم فروش حقیقی (تعداد سهام فروخته شده توسط افراد حقیقی)
//        /// </summary>
//        [JsonProperty("sell_I_Volume")]
//        public double SellIVolume { get; set; }

//        /// <summary>
//        /// تعداد خریداران حقیقی (تعداد کدهای حقیقی خریدار)
//        /// </summary>
//        [JsonProperty("buy_I_Count")]
//        public double BuyICount { get; set; }

//        /// <summary>
//        /// حجم فروش حقوقی (تعداد سهام فروخته شده توسط کدهای حقوقی)
//        /// </summary>
//        [JsonProperty("sell_N_Volume")]
//        public double SellNVolume { get; set; }

//        /// <summary>
//        /// ارزش فروش حقیقی (ارزش ریالی کل فروش افراد حقیقی)
//        /// </summary>
//        [JsonProperty("sell_I_Value")]
//        public double SellIValue { get; set; }

//        /// <summary>
//        /// ارزش فروش حقوقی (ارزش ریالی کل فروش کدهای حقوقی)
//        /// </summary>
//        [JsonProperty("sell_N_Value")]
//        public double SellNValue { get; set; }

//        /// <summary>
//        /// تعداد فروشندگان حقوقی (تعداد کدهای حقوقی فروشنده)
//        /// </summary>
//        [JsonProperty("sell_N_Count")]
//        public int SellNCount { get; set; }

//        /// <summary>
//        /// تعداد فروشندگان حقیقی (تعداد کدهای حقیقی فروشنده)
//        /// </summary>
//        [JsonProperty("sell_I_Count")]
//        public int SellICount { get; set; }
//    }
//}
