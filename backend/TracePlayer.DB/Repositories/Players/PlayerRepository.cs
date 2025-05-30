﻿using Microsoft.EntityFrameworkCore;
using TracePlayer.Contracts.Player;
using TracePlayer.DB.Helpers;
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
                        Names = new List<PlayerName> { new() { Name = dto.Name, Server = server } },
                        Ips = new List<PlayerIp> { new() { Ip = dto.Ip, CountryCode = dto.CountryCode } }
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

            if (playersToAdd.Count != 0)
            {
                await _context.Players.AddRangeAsync(playersToAdd);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Player?> Get(long playerId)
        {
            return await _context.Players
                .Where(p => p.Id == playerId)
                .Select(p => new Player
                {
                    Id = p.Id,
                    SteamId = p.SteamId,
                    SteamId64 = p.SteamId64,
                    Names = p.Names
                        .OrderBy(n => n.AddedAt)
                        .ToList(),
                    Ips = p.Ips
                        .Where(i => i.CountryCode != null)
                        .OrderByDescending(i => i.AddedAt)
                        .Take(10)
                        .ToList(),
                })
                .FirstOrDefaultAsync();
        }

        public async Task<long?> GetId(string steamId)
        {
            return await _context.Players
                .Where(p => p.SteamId == steamId)
                .Select(p => (long?)p.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<(List<GetPlayerPaginationResponse> Items, int TotalCount)> GetPaginated(string? search, int page, int pageSize)
        {
            var query = _context.Players
                .Include(p => p.Names)
                .Include(p => p.Ips)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.SteamId.Contains(search) ||
                    p.Names.Any(n => n.Name.Contains(search)));
            }

            query = query.OrderByDescending(p => p.Names
                .OrderByDescending(n => n.AddedAt)
                .Select(n => n.AddedAt)
                .FirstOrDefault());

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new GetPlayerPaginationResponse
                {
                    Id = p.Id,
                    SteamId = p.SteamId,
                    Name = p.Names.Select(n => n.Name).FirstOrDefault(),
                    CountryCode = p.Ips
                        .Where(ip => ip.CountryCode != null)
                        .Select(ip => ip.CountryCode)
                        .FirstOrDefault(),
                })
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task UpdateSteamId64()
        {
            var playersToUpdate = await _context.Players
                .Where(p => p.SteamId64 == null && p.SteamId != null)
                .ToListAsync();

            foreach (var player in playersToUpdate)
            {
                player.SteamId64 = SteamIdConverter.ConvertSteamIdToSteamId64(player.SteamId);
            }

            await _context.SaveChangesAsync();
        }
    }
}