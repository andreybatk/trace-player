using Microsoft.EntityFrameworkCore;
using TracePlayer.Contracts.Player;
using TracePlayer.DB.Models;

namespace TracePlayer.DB.Repositories.Players
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly ApplicationDbContext _context;

        public PlayerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddNames(List<AddPlayerDto> players, string server)
        {
            var playersToAdd = new List<Player>();

            foreach (var dto in players)
            {
                if (string.IsNullOrEmpty(dto.SteamId) || string.IsNullOrEmpty(dto.Ip) || string.IsNullOrEmpty(dto.Name))
                    continue;

                var player = playersToAdd.FirstOrDefault(p => p.SteamId == dto.SteamId);

                if (player is null)
                {
                    player = await _context.Players
                        .Include(p => p.Names)
                        .Include(p => p.Ips)
                        .FirstOrDefaultAsync(p => p.SteamId == dto.SteamId);
                }

                if (player is null)
                {
                    playersToAdd.Add(new Player
                    {
                        SteamId = dto.SteamId,
                        SteamId64 = dto.SteamId64,
                        Names = new List<PlayerName> { new PlayerName { Name = dto.Name, Server = server } },
                        Ips = new List<PlayerIp> { new PlayerIp { Ip = dto.Ip, CountryCode = dto.CountryCode } }
                    });
                }
                else
                {
                    if (!player.Names.Any(n => n.Name == dto.Name))
                    {
                        player.Names.Add(new PlayerName { Name = dto.Name, Server = server });
                    }

                    if (!player.Ips.Any(i => i.Ip == dto.Ip))
                    {
                        player.Ips.Add(new PlayerIp { Ip = dto.Ip, CountryCode = dto.CountryCode });
                    }
                }
            }

            if (playersToAdd.Any())
            {
                await _context.Players.AddRangeAsync(playersToAdd);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Player?> Get(long playerId)
        {
            return await _context.Players
                .Include(p => p.Names)
                .Include(p => p.Ips)
                .FirstOrDefaultAsync(p => p.Id == playerId);
        }

        public async Task<long?> GetId(string steamId)
        {
            return await _context.Players
                .Where(p => p.SteamId == steamId)
                .Select(p => (long?)p.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<(List<GetPlayerPaginationResponse> Items, int TotalCount)> GetPaginated(string? steamId, int page, int pageSize)
        {
            var query = _context.Players
                .Include(p => p.Names)
                .Include(p => p.Ips)
                .Where(p =>
                    (string.IsNullOrWhiteSpace(steamId) || p.SteamId == steamId));

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new GetPlayerPaginationResponse
                {
                    Id = p.Id,
                    SteamId = p.SteamId,
                    Name = p.Names.Select(n => n.Name).FirstOrDefault(),
                    CountryCode = p.Ips.Select(n => n.CountryCode).FirstOrDefault(),
                })
                .ToListAsync();

            return (items, totalCount);
        }
    }
}