using TracePlayer.Contracts.Player;
using TracePlayer.DB.Models;

namespace TracePlayer.DB.Repositories.Players
{
    public interface IPlayerRepository
    {
        Task AddNames(List<AddPlayerDto> players, string server);
        Task<Player?> Get(long playerId);
        Task<long?> GetId(string steamId);
        Task<(List<GetPlayerPaginationResponse> Items, int TotalCount)> GetPaginated(string? steamId, int page, int pageSize);
        Task UpdateSteamId64();
    }
}
