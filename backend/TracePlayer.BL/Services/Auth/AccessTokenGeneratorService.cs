using TracePlayer.BL.Helpers;
using TracePlayer.BL.Models;
using TracePlayer.DB.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace TracePlayer.BL.Services.Auth
{
    public class AccessTokenGeneratorService
    {
        private readonly AuthenticationConfiguration _configuration;
        private readonly TokenGeneratorService _tokenGeneratorService;
        private readonly UserManager<User> _userManager;

        public AccessTokenGeneratorService(AuthenticationConfiguration configuration, TokenGeneratorService tokenGeneratorService, UserManager<User> userManager)
        {
            _configuration = configuration;
            _tokenGeneratorService = tokenGeneratorService;
            _userManager = userManager;
        }

        public async Task<AccessToken> GenerateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            if (user?.Email is not null)
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }
            if (user?.UserName is not null)
            {
                claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            }

            if(user is not null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                    claims.Add(new Claim("role", role)); // custom ClaimType "role" for frontend
                }
            }

            DateTime expirationTime = DateTime.UtcNow.AddMinutes(_configuration.AccessTokenExpirationMinutes);
            string value = _tokenGeneratorService.GenerateToken(
                _configuration.AccessTokenSecret,
                _configuration.Issuer,
                _configuration.Audience,
                expirationTime,
                claims);

            return new AccessToken()
            {
                Value = value,
                ExpirationTime = expirationTime
            };
        }
    }
}