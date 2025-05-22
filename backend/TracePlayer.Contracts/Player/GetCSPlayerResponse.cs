using TracePlayer.Contracts.Steam;

namespace TracePlayer.Contracts.Player
{
    public class GetCSPlayerResponse
    {
        public string SteamId { get; set; } = string.Empty;
        public List<PlayerNameResponse> Names { get; set; } = [];
        public FullSteamPlayerInfo? FullSteamPlayerInfo { get; set; }
    }
}