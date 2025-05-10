using TracePlayer.BL.Models;
using TracePlayer.DB.Models;
using TracePlayer.DB.Repositories.Token;

namespace TracePlayer.BL.Services.Auth
{
    public class AuthenticatorService
    {
        private readonly AccessTokenGeneratorService _accessTokenGenerator;
        private readonly RefreshTokenGeneratorService _refreshTokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthenticatorService(AccessTokenGeneratorService accessTokenGenerator, RefreshTokenGeneratorService refreshTokenGenerator, IRefreshTokenRepository refreshTokenRepository)
        {
            _accessTokenGenerator = accessTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<AuthenticatedUser> AuthenticateAsync(User user)
        {
            var accessToken = await _accessTokenGenerator.GenerateToken(user);
            string refreshToken = _refreshTokenGenerator.GenerateToken();

            RefreshToken refreshTokenDTO = new RefreshToken()
            {
                Token = refreshToken,
                UserId = user.Id
            };

            await _refreshTokenRepository.Create(refreshTokenDTO);

            return new AuthenticatedUser()
            {
                AccessToken = accessToken.Value,
                AccessTokenExpirationTime = accessToken.ExpirationTime,
                RefreshToken = refreshToken
            };
        }
    }
}
