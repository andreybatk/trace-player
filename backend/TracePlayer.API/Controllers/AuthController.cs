using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TracePlayer.API.Validators;
using TracePlayer.BL.Helpers;
using TracePlayer.BL.Services.Auth;
using TracePlayer.Contracts.Auth;
using TracePlayer.DB.Models;
using TracePlayer.DB.Repositories.Token;

namespace TracePlayer.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RefreshTokenValidator _refreshTokenValidator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly AuthenticatorService _authenticatorService;

        public AuthController(UserManager<User> userManager, RefreshTokenValidator refreshTokenValidator, IRefreshTokenRepository refreshTokenRepository, AuthenticatorService authenticatorService)
        {
            _userManager = userManager;
            _refreshTokenValidator = refreshTokenValidator;
            _refreshTokenRepository = refreshTokenRepository;
            _authenticatorService = authenticatorService;
        }

        [HttpGet("steam-login")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
        public IActionResult SteamLogin(string returnUrl)
        {
            if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out var uri) ||
                !(uri.Host == "localhost" || uri.Host.EndsWith("frontend.com")))
            {
                return BadRequest("Invalid returnUrl.");
            }

            var redirectUrl = Url.Action(nameof(SteamCallback), "Auth", new { returnUrl });
            var props = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(props, "Steam");
        }

        [HttpGet("steam-callback")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SteamCallback(string returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (!result.Succeeded)
                return Unauthorized("Steam authentication failed.");

            var fullSteamId64 = result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(fullSteamId64))
                return BadRequest("FullSteamID64 not found.");

            var steamId64 = SteamIdConverter.ExtractSteamId64FromOpenId(fullSteamId64);
            if (string.IsNullOrEmpty(steamId64))
                return BadRequest("Failed to extract SteamID64");

            var steamId = SteamIdConverter.ConvertSteamId64ToSteamId(steamId64);
            if (string.IsNullOrEmpty(steamId))
                return BadRequest("Failed to convert SteamID64 to SteamID");

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.SteamId64 == steamId64);

            if (user == null)
            {
                user = new User
                {
                    UserName = $"steam_{steamId64}",
                    SteamId64 = steamId64,
                    SteamId = steamId,
                    Email = $"{steamId64}@steam.local"
                };

                var identityResult = await _userManager.CreateAsync(user);
                if (!identityResult.Succeeded)
                    return BadRequest(identityResult.Errors);
            }

            var authUser = await _authenticatorService.AuthenticateAsync(user);
            var response = new AuthenticatedUserResponse
            {
                AccessToken = authUser.AccessToken,
                RefreshToken = authUser.RefreshToken,
                AccessTokenExpirationTime = authUser.AccessTokenExpirationTime
            };
            return Redirect($"{returnUrl}?accessToken={response.AccessToken}&refreshToken={response.RefreshToken}");
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthenticatedUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest refreshRequest)
        {
            bool isValidRefreshToken = _refreshTokenValidator.Validate(refreshRequest.RefreshToken);
            if (!isValidRefreshToken)
            {
                ModelState.AddModelError(nameof(refreshRequest.RefreshToken), "Invalid refresh token.");
                return BadRequest(ModelState);
            }

            var refreshTokenDTO = await _refreshTokenRepository.GetByToken(refreshRequest.RefreshToken);
            if (refreshTokenDTO == null)
            {
                ModelState.AddModelError(nameof(refreshRequest.RefreshToken), "Invalid refresh token.");
                return BadRequest(ModelState);
            }

            await _refreshTokenRepository.Delete(refreshTokenDTO.Id);

            var user = await _userManager.FindByIdAsync(refreshTokenDTO.UserId.ToString());
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return BadRequest(ModelState);
            }

            var authUser = await _authenticatorService.AuthenticateAsync(user);

            var response = new AuthenticatedUserResponse
            {
                AccessToken = authUser.AccessToken,
                RefreshToken = authUser.RefreshToken,
                AccessTokenExpirationTime = authUser.AccessTokenExpirationTime
            };

            return Ok(response);
        }


        [Authorize]
        [HttpDelete("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized();
            }

            await _refreshTokenRepository.DeleteAll(userId);

            return NoContent();
        }
    }
}