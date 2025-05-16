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

        public async Task<ServiceResult<GetCSPlayerResponse?>> GetCSPlayerResponse(string steamId)
        {
            var id = await _playerRepository.GetId(steamId);
            if (id is null)
            {
                return ServiceResult<GetCSPlayerResponse?>.Fail("Player profile not found for this user.");
            }

            var player = await _playerRepository.Get(id.Value);

            if (player is null)
            {
                return ServiceResult<GetCSPlayerResponse?>.Fail($"Player with id {id} not found.");
            }

            FullSteamPlayerInfo? fullSteamPlayerInfo = null;
            if (!string.IsNullOrEmpty(player.SteamId64))
            {
                fullSteamPlayerInfo = await _steamApiService.GetFullSteamPlayerInfoAsync(player.SteamId64);
                if(fullSteamPlayerInfo?.PlayerInfo is not null)
                {
                    fullSteamPlayerInfo.PlayerInfo.Profileurl = fullSteamPlayerInfo.PlayerInfo.Profileurl.Replace("https", "http");
                    fullSteamPlayerInfo.PlayerInfo.Avatarfull = fullSteamPlayerInfo.PlayerInfo.Avatarfull.Replace("https", "http");
                }
            }

            var namesRows = string.Join("\n", player.Names.Select(n =>
                $"<tr><td>{n.Name}</td><td>{n.Server}</td><td>{n.AddedAt:dd.MM.yyyy}</td></tr>"));

            var ipsRows = string.Join("\n", player.Ips.Select(ip =>
                $"<tr><td>{ip.CountryCode}</td><td>{ip.AddedAt:dd.MM.yyyy}</td></tr>"));

            var response = new GetCSPlayerResponse
            {
                SteamId = player.SteamId,
                FullSteamPlayerInfo = fullSteamPlayerInfo,
                NamesRows = namesRows,
                IpsRows = ipsRows
            };

            return ServiceResult<GetCSPlayerResponse?>.Ok(response);
        }

        public async Task<ServiceResult<GetPlayerResponse?>> GetPlayerResponse(long id)
        {
            var player = await _playerRepository.Get(id);

            if (player is null)
            {
                return ServiceResult<GetPlayerResponse?>.Fail($"Player with id {id} not found.");
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
    }
}
