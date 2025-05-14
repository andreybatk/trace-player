using TracePlayer.Contracts.Fungun;
using TracePlayer.Contracts.Steam;

namespace TracePlayer.Contracts.Player
{
    public class GetPlayerResponse
    {
        public string SteamId { get; set; } = string.Empty;
        public List<PlayerIpResponse> Ips { get; set; } = [];
        public List<PlayerNameResponse> Names { get; set; } = [];
        public FullSteamPlayerInfo? FullSteamPlayerInfo { get; set; }
    }
}
