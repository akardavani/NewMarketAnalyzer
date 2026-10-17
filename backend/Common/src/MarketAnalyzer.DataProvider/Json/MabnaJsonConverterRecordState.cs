using MarketAnalyzer.DataProvider.MabnaModels;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketAnalyzer.DataProvider.Json
{
    internal class MabnaJsonConverterRecordState : JsonConverter<RecordState>
    {
        public override RecordState Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return value switch
            {
                "inserted" => RecordState.Inserted,
                "updated" => RecordState.Updated,
                "deleted" => RecordState.Deleted,
                _ => throw new NotSupportedException(value),
            };
        }

        public override void Write(Utf8JsonWriter writer, RecordState value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
