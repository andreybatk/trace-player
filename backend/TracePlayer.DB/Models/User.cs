using Microsoft.AspNetCore.Identity;

namespace TracePlayer.DB.Models
{
    public class User : IdentityUser<Guid>
    {
        public string? SteamId64 { get; set; }
        public string? SteamId { get; set; }
    }
}