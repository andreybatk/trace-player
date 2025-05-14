namespace TracePlayer.Contracts.Steam
{
    public class FullSteamPlayerInfo
    {
        public SteamPlayerInfo? PlayerInfo { get; set; }
        public SteamPlayerBanInfo? BanInfo { get; set; }
    }
}
