namespace TracePlayer.DB.Models
{
    public class PlayerIp
    {
        public long Id { get; set; }
        public string Ip { get; set; } = string.Empty;
        public string? CountryCode { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public long PlayerId { get; set; }
        public Player Player { get; set; } = null!;
    }
}
