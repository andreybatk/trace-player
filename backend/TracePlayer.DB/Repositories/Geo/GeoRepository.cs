using Microsoft.EntityFrameworkCore;

namespace TracePlayer.DB.Repositories.Geo
{
    public class GeoRepository : IGeoRepository
    {
        private readonly ApplicationDbContext _context;

        public GeoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string?> GetCountryCode(long ipLong)
        {
            return await _context.IpCountries.Where(i => i.IpFrom < ipLong && ipLong < i.IpTo).Select(i => i.CountryCode).FirstOrDefaultAsync();
        }
    }
}
