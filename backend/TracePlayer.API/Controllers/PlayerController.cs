using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TracePlayer.BL.Helpers;
using TracePlayer.BL.Services.Fungun;
using TracePlayer.BL.Services.Geo;
using TracePlayer.BL.Services.Steam;
using TracePlayer.Contracts.Fungun;
using TracePlayer.Contracts.Player;
using TracePlayer.Contracts.Steam;
using TracePlayer.DB.Repositories.Players;
using TracePlayer.DB.Repositories.Users;

namespace TracePlayer.API.Controllers
{
    [Route("api/player")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IUserRepository _userRepository;
        private readonly SteamApiService _steamApiService;
        private readonly FungunApiService _fungunApiService;
        private readonly GeoService _geoService;

        public PlayerController(IPlayerRepository playerRepository, IUserRepository userRepository, SteamApiService steamApiService, FungunApiService fungunApiService, GeoService geoService)
        {
            _playerRepository = playerRepository;
            _userRepository = userRepository;
            _steamApiService = steamApiService;
            _fungunApiService = fungunApiService;
            _geoService = geoService;
        }

        //[ApiKeyAuth]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddNames([FromBody] AddPlayersNamesRequest request)
        {
            var players = request.Players.Select(p => new AddPlayerDto
            {
                SteamId = p.SteamId,
                SteamId64 = SteamIdConverter.ConvertSteamIdToSteamId64(p.SteamId),
                Ip = p.Ip,
                Name = p.Name,
                CountryCode = _geoService.GetCountryCode(p.Ip)
            }).ToList();

            await _playerRepository.AddNames(players, request.Server, request.ServerIp);
            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(long?), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyPlayer()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized();
            }

            var steamId = await _userRepository.GetSteamId(userId);
            if (steamId is null)
            {
                return NotFound("Player not linked to this user.");
            }

            return Ok(await _playerRepository.GetId(steamId));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetPlayerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPlayer([FromRoute] long id)
        {
            var player = await _playerRepository.Get(id);

            if (player is null)
            {
                return NotFound($"Player with id {id} not found.");
            }

            SteamPlayerInfo? steamPlayerInfo = null;
            if (!string.IsNullOrEmpty(player.SteamId64))
            {
                steamPlayerInfo = await _steamApiService.GetPlayerInfoAsync(player.SteamId64);
            }
            
            var response = new GetPlayerResponse
            {
                SteamId = player.SteamId,
                SteamPlayerInfo = steamPlayerInfo,
                Ips = player.Ips.Select(ip => new PlayerIpResponse
                {
                    CountryCode = ip.CountryCode,
                    AddedAt = ip.AddedAt
                }).ToList(),
                Names = player.Names.Select(name => new PlayerNameResponse
                {
                    Name = name.Name,
                    AddedAt = name.AddedAt,
                    Server = name.Server,
                    ServerIp = name.ServerIp
                }).ToList()
            };

            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PlayersWithTotalCountResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaginated([FromQuery] GetPlayersPaginationRequest request)
        {
            var (Items, TotalCount) = await _playerRepository.GetPaginated(
                request.SteamId,
                request.Page,
                request.PageSize
            );

            var response = new PlayersWithTotalCountResponse
            {
                Players = Items,
                TotalCount = TotalCount
            };

            return Ok(response);
        }
    }
}