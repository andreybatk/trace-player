using TracePlayer.Contracts.Fungun;
using TracePlayer.Contracts.Steam;

namespace TracePlayer.Contracts.Player
{
    public class GetCSPlayerResponse
    {
        public string SteamId { get; set; } = string.Empty;
        public string? CountryCode { get; set; }
        public List<PlayerNameResponse> Names { get; set; } = [];
        public FullSteamPlayerInfo? FullSteamPlayerInfo { get; set; }
        public FungunPlayer? FungunPlayer { get; set; }
    }
}