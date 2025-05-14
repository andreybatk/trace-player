namespace TracePlayer.Contracts.Steam
{
    public class SteamPlayerBanInfo
    {
        public bool CommunityBanned { get; set; }
        public int NumberOfVACBans { get; set; }
        public int NumberOfGameBans { get; set; }
    }
}
