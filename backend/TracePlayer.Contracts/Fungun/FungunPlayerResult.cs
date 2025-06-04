using System.Text.Json.Serialization;

namespace TracePlayer.Contracts.Fungun
{
    public class FungunPlayerResult
    {
        [JsonPropertyName("report_id")]
        public int ReportId { get; set; }

        [JsonPropertyName("steamid_game")]
        public long SteamIdGame { get; set; }

        [JsonPropertyName("result_status")]
        public string ResultStatus { get; set; } = string.Empty;
        [JsonPropertyName("uuid")]
        public int Uuid { get; set; }
    }
}