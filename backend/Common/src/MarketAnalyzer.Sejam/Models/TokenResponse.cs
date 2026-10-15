using System;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.Sejam.Models
{
    public class TokenResponse
    {
        /// <summary>
        /// توکن دسترسی به سرویسها
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// مدت زمان اعتبار توکن
        /// </summary>
        [JsonPropertyName("ttl")]
        public TimeSpan Timelife { get; set; }
    }
}