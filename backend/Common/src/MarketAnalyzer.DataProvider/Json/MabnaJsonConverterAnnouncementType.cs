using MarketAnalyzer.DataProvider.MabnaModels;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.DataProvider.Json
{
    internal class MabnaJsonConverterAnnouncementType : JsonConverter<AnnouncementType>
    {
        public override AnnouncementType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return value switch
            {
                "meeting" => AnnouncementType.Meeting,
                "forecast" => AnnouncementType.Forecast,
                "midterm" => AnnouncementType.Midterm,
                _ => throw new NotSupportedException(value),
            };
        }

        public override void Write(Utf8JsonWriter writer, AnnouncementType value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
