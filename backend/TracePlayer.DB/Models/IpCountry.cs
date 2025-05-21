namespace TracePlayer.DB.Models
{
    public class IpCountry
    {
        public long IpFrom { get; set; }
        public long IpTo { get; set; }
        public string CountryCode { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
    }
}
