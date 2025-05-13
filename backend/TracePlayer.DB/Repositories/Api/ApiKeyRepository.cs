using Microsoft.EntityFrameworkCore;
using TracePlayer.DB.Models;

namespace TracePlayer.DB.Repositories.Api
{
    public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly ApplicationDbContext _context;

        public ApiKeyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddKeyHash(string serverIp, string keyHash)
        {
            var exists = await _context.ApiKeys.AnyAsync(k => k.ServerIp == serverIp);
            if (exists)
                return false;

            var apiKey = new ApiKey
            {
                KeyHash = keyHash,
                ServerIp = serverIp
            };

            _context.ApiKeys.Add(apiKey);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsValidKeyHash(string keyHash)
        {
            return await _context.ApiKeys.AnyAsync(k => k.KeyHash == keyHash);
        }

        public async Task<bool> IsValidServerIp(string serverIp)
        {
            return await _context.ApiKeys.AnyAsync(k => k.ServerIp == serverIp);
        }

        public async Task<List<string>> GetAllServerIps()
        {
            return await _context.ApiKeys
                .Select(k => k.ServerIp)
                .ToListAsync();
        }

        public async Task<bool> DeleteApiKeyByServerIp(string serverIp)
        {
            var apiKey = await _context.ApiKeys
                .FirstOrDefaultAsync(k => k.ServerIp == serverIp);

            if (apiKey == null)
                return false;

            _context.ApiKeys.Remove(apiKey);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
