namespace TracePlayer.DB.Helpers
{
    public static class SteamIdConverter
    {
        public static string ConvertSteamId64ToSteamId(string steamId64)
        {
            long id = long.Parse(steamId64);
            long z = id - 76561197960265728;
            long y = z % 2;
            long x = 0;
            long a = (z - y) / 2;

            return $"STEAM_{x}:{y}:{a}";
        }

        public static string? ConvertSteamIdToSteamId64(string steamId)
        {
            if (!steamId.StartsWith("STEAM_")) return null;

            var parts = steamId.Substring(6).Split(':');
            if (parts.Length != 3) return null;

            if (!int.TryParse(parts[0], out int universe)) return null;
            if (!int.TryParse(parts[1], out int y)) return null;
            if (!int.TryParse(parts[2], out int z)) return null;

            long steamId64 = (z * 2L) + y + 76561197960265728;
            return steamId64.ToString();
        }

        public static string? ExtractSteamId64FromOpenId(string openIdClaim)
        {
            var prefix = "https://steamcommunity.com/openid/id/";
            if (openIdClaim.StartsWith(prefix))
                return openIdClaim.Substring(prefix.Length);

            return null;
        }
    }
}