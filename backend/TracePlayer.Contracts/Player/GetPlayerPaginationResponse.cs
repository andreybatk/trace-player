namespace TracePlayer.Contracts.Player
{
    public class GetPlayerPaginationResponse
    {
        public long Id { get; set; }
        public string SteamId { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? CountryCode { get; set; }
    }
}
