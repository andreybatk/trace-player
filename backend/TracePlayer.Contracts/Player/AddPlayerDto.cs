namespace TracePlayer.Contracts.Player
{
    public class AddPlayerDto
    {
        public string SteamId { get; set; } = null!;
        public string? SteamId64 { get; set; }
        public string Ip { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? CountryCode { get; set; }
    }
}
