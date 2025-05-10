using TracePlayer.BL.Helpers;

namespace TracePlayer.BL.Services.Auth
{
    public class RefreshTokenGeneratorService
    {
        private readonly AuthenticationConfiguration _configuration;
        private readonly TokenGeneratorService _tokenGeneratorService;

        public RefreshTokenGeneratorService(AuthenticationConfiguration authenticationConfiguration, TokenGeneratorService tokenGeneratorService)
        {
            _configuration = authenticationConfiguration;
            _tokenGeneratorService = tokenGeneratorService;
        }

        public string GenerateToken()
        {
            DateTime expirationTime = DateTime.UtcNow.AddMinutes(_configuration.RefreshTokenExpirationMinutes);

            return _tokenGeneratorService.GenerateToken(
                _configuration.RefreshTokenSecret,
                _configuration.Issuer,
                _configuration.Audience,
                expirationTime);
        }
    }
}