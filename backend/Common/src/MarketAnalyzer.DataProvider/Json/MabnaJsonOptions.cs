using System.Text.Json;

namespace MarketAnalyzer.DataProvider.Json
{
    internal class MabnaJsonOptions
    {
        public static readonly MabnaJsonOptions Default = new();

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public MabnaJsonOptions()
        {
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy()
            };
            _jsonSerializerOptions.Converters.Insert(0, new MabnaJsonConverterStringConverter());
            _jsonSerializerOptions.Converters.Insert(0, new MabnaJsonConverterDateTime());
            _jsonSerializerOptions.Converters.Insert(0, new MabnaJsonConverterRecordState());
            _jsonSerializerOptions.Converters.Insert(0, new MabnaJsonConverterAnnouncementType());
        }

        public JsonSerializerOptions SerializerOptions => _jsonSerializerOptions;
    }
}
