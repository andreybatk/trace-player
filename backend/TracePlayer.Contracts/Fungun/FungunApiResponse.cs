using System.Text.Json.Serialization;
using TracePlayer.Contracts.Helpers;

namespace TracePlayer.Contracts.Fungun
{
    public class FungunApiResponse
    {
        [JsonPropertyName("data")]
        [JsonConverter(typeof(FungunDataConverter))]
        public Dictionary<string, List<FungunPlayerResult>> Data { get; set; } = [];
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}