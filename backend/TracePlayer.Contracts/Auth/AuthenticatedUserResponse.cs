namespace TracePlayer.Contracts.Auth
{
    public class AuthenticatedUserResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpirationTime { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}
