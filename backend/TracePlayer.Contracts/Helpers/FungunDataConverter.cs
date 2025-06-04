using System.Text.Json;
using System.Text.Json.Serialization;
using TracePlayer.Contracts.Fungun;

namespace TracePlayer.Contracts.Helpers
{
    public class FungunDataConverter : JsonConverter<Dictionary<string, List<FungunPlayerResult>>>
    {
        public override Dictionary<string, List<FungunPlayerResult>> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                return JsonSerializer.Deserialize<Dictionary<string, List<FungunPlayerResult>>>(ref reader, options)
                       ?? new Dictionary<string, List<FungunPlayerResult>>();
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                // Если data — пустой массив []
                reader.Read();
                if (reader.TokenType == JsonTokenType.EndArray)
                    return new Dictionary<string, List<FungunPlayerResult>>();
            }

            throw new JsonException("Unexpected JSON for 'data' field.");
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, List<FungunPlayerResult>> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}