using TracePlayer.BL.Helpers;
using TracePlayer.BL.Services.Fungun;
using TracePlayer.BL.Services.Geo;
using TracePlayer.BL.Services.Steam;
using TracePlayer.Contracts.Player;
using TracePlayer.Contracts.Steam;
using TracePlayer.DB.Repositories.Players;
using TracePlayer.DB.Repositories.Users;

namespace TracePlayer.BL.Services.Player
{
    public class PlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IUserRepository _userRepository;
        private readonly SteamApiService _steamApiService;
        private readonly FungunApiService _fungunApiService;
        private readonly GeoService _geoService;

        public PlayerService(IPlayerRepository playerRepository, IUserRepository userRepository, SteamApiService steamApiService, FungunApiService fungunApiService, GeoService geoService)
        {
            _playerRepository = playerRepository;
            _userRepository = userRepository;
            _steamApiService = steamApiService;
            _fungunApiService = fungunApiService;
            _geoService = geoService;
        }

        public async Task AddNames(AddPlayersNamesRequest request)
        {
            var players = request.Players.Select(p => new AddPlayerDto
            {
                SteamId = p.SteamId,
                SteamId64 = SteamIdConverter.ConvertSteamIdToSteamId64(p.SteamId),
                Ip = p.Ip,
                Name = p.Name,
                CountryCode = _geoService.GetCountryCode(p.Ip)
            }).ToList();

            await _playerRepository.AddNames(players, request.Server);
        }

        public async Task<ServiceResult<long>> GetIdByUserId(Guid userId)
        {
            var steamId = await _userRepository.GetSteamId(userId);
            if (steamId is null)
            {
                return ServiceResult<long>.Fail("Player is not associated with this user.");
            }

            var id = await _playerRepository.GetId(steamId);
            if (id is null)
            {
                return ServiceResult<long>.Fail("Player profile not found for this user.");
            }

            return ServiceResult<long>.Ok(id.Value);
        }

        public async Task<ServiceResult<GetPlayerResponse?>> GetPlayerResponse(long id)
        {
            var player = await _playerRepository.Get(id);

            if (player is null)
            {
                return ServiceResult<GetPlayerResponse?>.Fail("Player with id {id} not found.");
            }

            FullSteamPlayerInfo? fullSteamPlayerInfo = null;
            if (!string.IsNullOrEmpty(player.SteamId64))
            {
                fullSteamPlayerInfo = await _steamApiService.GetFullSteamPlayerInfoAsync(player.SteamId64);
            }

            var response = new GetPlayerResponse
            {
                SteamId = player.SteamId,
                FullSteamPlayerInfo = fullSteamPlayerInfo,
                Ips = player.Ips.Select(ip => new PlayerIpResponse
                {
                    CountryCode = ip.CountryCode,
                    AddedAt = ip.AddedAt
                }).ToList(),
                Names = player.Names.Select(name => new PlayerNameResponse
                {
                    Name = name.Name,
                    AddedAt = name.AddedAt,
                    Server = name.Server
                }).ToList()
            };

            return ServiceResult<GetPlayerResponse?>.Ok(response);
        }

        public async Task<ServiceResult<PlayersWithTotalCountResponse>> GetPaginated(GetPlayersPaginationRequest request)
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

            return ServiceResult<PlayersWithTotalCountResponse>.Ok(response);
        }

        public async Task<ServiceResult<string>> GetMotdHtml(string steamId)
        {
            //var template = await System.IO.File.ReadAllTextAsync("wwwroot/templates/player-motd-template.html");
            var template = await System.IO.File.ReadAllTextAsync("templates/player-motd-template.html");

            if(template is null)
            {
                return ServiceResult<string>.Fail("Template fot MotdHtml not found.");
            }
            var id = await _playerRepository.GetId(steamId);

            if (id is null)
            {
                var templateNotFound = await System.IO.File.ReadAllTextAsync("templates/player-not-found-motd-template.html");
                return ServiceResult<string>.Ok(templateNotFound);
            }

            var result = await GetPlayerResponse(id.Value);

            if (!result.Success)
            {
                return ServiceResult<string>.Fail(result.ErrorMessage);
            }

            if (result.Data is null)
            {
                var templateNotFound = await System.IO.File.ReadAllTextAsync("templates/player-not-found-motd-template.html");
                return ServiceResult<string>.Ok(templateNotFound);
            }

            var namesRows = string.Join("\n", result.Data.Names.Select(n =>
                $"<tr><td>{n.Name}</td><td>{n.Server}</td><td>{n.AddedAt:dd.MM.yyyy}</td></tr>"));

            var ipsRows = string.Join("\n", result.Data.Ips.Select(ip =>
                $"<tr><td>{ip.CountryCode}</td><td>{ip.AddedAt:dd.MM.yyyy}</td></tr>"));

            var playerInfo = result.Data.FullSteamPlayerInfo?.PlayerInfo;
            var banInfo = result.Data.FullSteamPlayerInfo?.BanInfo;

            var html = template
                .Replace("{{personaname}}", playerInfo?.Personaname ?? "Неизвестно")
                .Replace("{{avatarfull}}", playerInfo?.Avatarfull ?? "")
                .Replace("{{profileurl}}", playerInfo?.Profileurl ?? "#")
                .Replace("{{steamid}}", result.Data.SteamId)
                .Replace("{{vacBans}}", banInfo?.NumberOfVACBans.ToString() ?? "0")
                .Replace("{{gameBans}}", banInfo?.NumberOfGameBans.ToString() ?? "0")
                .Replace("{{communityBan}}", banInfo?.CommunityBanned == true ? "Community ban" : "")
                .Replace("{{namesRows}}", namesRows)
                .Replace("{{ipsRows}}", ipsRows);

            return ServiceResult<string>.Ok(html);
        }
    }
}
