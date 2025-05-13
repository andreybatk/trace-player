namespace TracePlayer.DB.Models
{
    public class ApiKey
    {
        public long Id { get; set; }
        public string KeyHash { get; set; } = string.Empty;
        public string ServerIp { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
