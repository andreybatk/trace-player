using TracePlayer.Contracts.Steam;

namespace TracePlayer.Contracts.Player
{
    public class GetCSPlayerResponse
    {
        public string SteamId { get; set; } = string.Empty;
        public string NamesRows { get; set; } = string.Empty;
        public FullSteamPlayerInfo? FullSteamPlayerInfo { get; set; }
    }
}