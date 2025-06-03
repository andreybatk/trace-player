using System.Text.Json.Serialization;

namespace TracePlayer.Contracts.Fungun
{
    public class FungunPlayerResult
    {
        [JsonPropertyName("report_id")]
        public int ReportId { get; set; }

        [JsonPropertyName("steamid_game")]
        public string SteamIdGame { get; set; } = string.Empty;

        [JsonPropertyName("result_status")]
        public string ResultStatus { get; set; } = string.Empty;
        public int Uuid { get; set; }
    }
}