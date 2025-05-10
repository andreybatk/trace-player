using System.ComponentModel.DataAnnotations;

namespace TracePlayer.Contracts.Player
{
    public class AddPlayerNameRequest
    {
        [Required]
        public string SteamId { get; set; } = string.Empty;
        [Required]
        public string Ip { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}