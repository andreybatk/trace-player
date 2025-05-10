using Microsoft.EntityFrameworkCore;

namespace TracePlayer.DB.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string?> GetSteamId(Guid id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Select(u => u.SteamId)
                .FirstOrDefaultAsync();
        }
    }
}
