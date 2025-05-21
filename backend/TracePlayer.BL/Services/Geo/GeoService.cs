
using Microsoft.Extensions.Logging;
using TracePlayer.DB.Repositories.Geo;

namespace TracePlayer.BL.Services.Geo
{
    public class GeoService
    {
        private readonly IGeoRepository _geoRepository;
        private readonly ILogger<GeoService> _logger;

        public GeoService(IGeoRepository geoRepository, ILogger<GeoService> logger)
        {
            _geoRepository = geoRepository;
            _logger = logger;
        }

        public async Task<string?> GetCountryCode(string ip)
        {
            var ipLong = GeoServiceHelper.IpToLong(ip);

            if(ipLong == 0)
                return null;

            return await _geoRepository.GetCountryCode(ipLong);
        }
    }
}
