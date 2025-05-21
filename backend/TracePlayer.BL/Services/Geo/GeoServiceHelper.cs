namespace TracePlayer.BL.Services.Geo
{
    public static class GeoServiceHelper
    {
        public static long IpToLong(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return 0;

            var segments = ip.Split('.');
            if (segments.Length != 4)
                return 0;

            return ((long.Parse(segments[0]) << 24) & 0xFF000000L) |
                   ((long.Parse(segments[1]) << 16) & 0x00FF0000L) |
                   ((long.Parse(segments[2]) << 8) & 0x0000FF00L) |
                   (long.Parse(segments[3]) & 0x000000FFL);
        }
    }
}