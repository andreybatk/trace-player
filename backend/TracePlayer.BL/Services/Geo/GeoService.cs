using MaxMind.GeoIP2;

namespace TracePlayer.BL.Services.Geo
{
    public class GeoService
    {
        private readonly DatabaseReader _reader;

        public GeoService()
        {
            _reader = new DatabaseReader("GeoLite2-Country.mmdb");
        }

        public string? GetCountryCode(string ip)
        {
            try
            {
                var country = _reader.Country(ip);
                return country?.Country?.IsoCode;
            }
            catch
            {
                return null;
            }
        }
    }
}
