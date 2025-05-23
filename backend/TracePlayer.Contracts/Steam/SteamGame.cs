using System.Text.Json.Serialization;

namespace TracePlayer.Contracts.Steam
{
    public class SteamGame
    {
        [JsonPropertyName("playtime_forever")]
        public int PlaytimeMinutes { get; set; }
        [JsonPropertyName("playtime_2weeks")]
        public int PlaytimeTwoWeeksMinutes { get; set; }
    }
}