using System.Text.Json.Serialization;

namespace TracePlayer.Contracts.Steam
{
    public class SteamOwnedGamesResponse
    {
        [JsonPropertyName("game_count")]
        public int GameCount { get; set; }
        public List<SteamGame>? Games { get; set; }
    }
}
