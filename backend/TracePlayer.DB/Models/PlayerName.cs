namespace TracePlayer.DB.Models
{
    public class PlayerName
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public string Server { get; set; } = string.Empty;

        public long PlayerId { get; set; }
        public Player Player { get; set; } = null!;
    }
}