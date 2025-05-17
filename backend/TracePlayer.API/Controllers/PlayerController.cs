using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TracePlayer.BL.Services.Player;
using TracePlayer.Contracts.Player;

namespace TracePlayer.API.Controllers
{
    [Route("api/player")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly PlayerService _playerService;
        private readonly ILogger<PlayerController> _logger;

        public PlayerController(PlayerService playerService, ILogger<PlayerController> logger)
        {
            _playerService = playerService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddNames([FromBody] AddPlayersNamesRequest request)
        {
            await _playerService.AddNames(request);
            _logger.LogInformation("Successful request to add players from: {server}", request.Server);
            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetIdByUserId()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized();
            }

            var result = await _playerService.GetIdByUserId(userId);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetPlayerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPlayer([FromRoute] long id)
        {
            var result = await _playerService.GetPlayerResponse(id);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        [HttpGet("bySteamId")]
        [ProducesResponseType(typeof(GetCSPlayerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPlayer([FromQuery] string steamId)
        {
            var result = await _playerService.GetCSPlayerResponse(steamId);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PlayersWithTotalCountResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaginated([FromQuery] GetPlayersPaginationRequest request)
        {
            var result = await _playerService.GetPaginated(request);

            return Ok(result.Data);
        }

    }
}