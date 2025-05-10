using TracePlayer.DB.Models;

namespace TracePlayer.DB.Models
{
    public class Player
    {
        public long Id { get; set; }
        public string? SteamId64 { get; set; }
        public string SteamId { get; set; } = string.Empty;
        public List<PlayerIp> Ips { get; set; } = [];
        public List<PlayerName> Names { get; set; } = [];
    }
}
