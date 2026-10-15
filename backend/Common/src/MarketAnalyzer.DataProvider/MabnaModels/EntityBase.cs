using System.Text.Json.Serialization;

namespace MarketAnalyzer.DataProvider.MabnaModels
{
    public abstract class EntityBase
    {
        /// <summary>
        /// کد رکورد
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// اطلاعات رکورد
        /// </summary>
        public Metadata Meta { get; set; }

        [JsonIgnore]
        public long LongId => long.Parse(Id);
    }
}
