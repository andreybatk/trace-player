namespace TracePlayer.Contracts.Player
{
    public class PlayerIpResponse
    {
        public string? CountryCode { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
