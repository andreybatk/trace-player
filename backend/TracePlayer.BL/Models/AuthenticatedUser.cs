namespace TracePlayer.BL.Models
{
    public class AuthenticatedUser
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpirationTime { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}