namespace MarketAnalyzer.AnalysisStrategy
{
    using System.Text.Json;
    public static class ParameterHelper
    {
        public static T GetRequiredParameter<T>(
            AnalysisRequest request,
            string key)
        {
            if (!request.Parameters.TryGetValue(key, out var value) || value is null)
                throw new ArgumentException($"Required parameter '{key}' was not provided.");

            if (value is T typedValue)
                return typedValue;

            if (value is JsonElement jsonElement)
            {
                return ConvertJsonElement<T>(jsonElement, key);
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static List<string> GetRequiredStringListParameter(
            AnalysisRequest request,
            string key)
        {
            if (!request.Parameters.TryGetValue(key, out var value) || value is null)
                throw new ArgumentException($"Required parameter '{key}' was not provided.");

            var result = ConvertToStringList(value, key);

            if (result.Count == 0)
                throw new ArgumentException($"Required parameter '{key}' was empty.");

            return result;
        }

        public static List<string> GetOptionalStringListParameter(
            AnalysisRequest request,
            string key)
        {
            if (!request.Parameters.TryGetValue(key, out var value) || value is null)
                return new List<string>();

            return ConvertToStringList(value, key);
        }

        private static List<string> ConvertToStringList(object value, string key)
        {
            if (value is List<string> stringList)
                return stringList;

            if (value is string[] stringArray)
                return stringArray.ToList();

            if (value is IEnumerable<string> stringEnumerable)
                return stringEnumerable.ToList();

            if (value is string singleString)
            {
                if (string.IsNullOrWhiteSpace(singleString))
                    return new List<string>();

                return singleString
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList();
            }

            if (value is JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == JsonValueKind.Array)
                {
                    return jsonElement
                        .EnumerateArray()
                        .Select(x => x.GetString())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x!)
                        .ToList();
                }

                if (jsonElement.ValueKind == JsonValueKind.String)
                {
                    var text = jsonElement.GetString();

                    if (string.IsNullOrWhiteSpace(text))
                        return new List<string>();

                    return text
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .ToList();
                }
            }

            throw new ArgumentException(
                $"Parameter '{key}' could not be converted to List<string>. Actual type: {value.GetType().Name}");
        }

        private static T ConvertJsonElement<T>(JsonElement jsonElement, string key)
        {
            try
            {
                var targetType = typeof(T);

                if (targetType == typeof(string))
                {
                    return (T)(object)(jsonElement.GetString() ?? "");
                }

                if (targetType == typeof(int))
                {
                    return (T)(object)jsonElement.GetInt32();
                }

                if (targetType == typeof(long))
                {
                    return (T)(object)jsonElement.GetInt64();
                }

                if (targetType == typeof(decimal))
                {
                    return (T)(object)jsonElement.GetDecimal();
                }

                if (targetType == typeof(double))
                {
                    return (T)(object)jsonElement.GetDouble();
                }

                if (targetType == typeof(bool))
                {
                    return (T)(object)jsonElement.GetBoolean();
                }

                if (targetType == typeof(DateTime))
                {
                    var text = jsonElement.GetString();

                    if (DateTime.TryParse(text, out var dateTime))
                        return (T)(object)dateTime;

                    throw new ArgumentException($"Parameter '{key}' could not be converted to DateTime.");
                }

                var deserialized = jsonElement.Deserialize<T>();

                if (deserialized is null)
                    throw new ArgumentException($"Parameter '{key}' could not be converted to {typeof(T).Name}.");

                return deserialized;
            }
            catch (Exception ex) when (ex is not ArgumentException)
            {
                throw new ArgumentException(
                    $"Parameter '{key}' could not be converted to {typeof(T).Name}.", ex);
            }
        }
    }

}
