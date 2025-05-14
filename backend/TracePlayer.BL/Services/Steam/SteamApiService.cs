using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using TracePlayer.Contracts.Steam;

namespace TracePlayer.BL.Services.Steam
{
    public class SteamApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SteamApiService> _logger;
        private readonly string _apiKey;

        public SteamApiService(HttpClient httpClient, IMemoryCache cache, ILogger<SteamApiService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
            _apiKey = configuration["Authentication:Steam:ApiKey"] ?? throw new InvalidOperationException("Steam API key is missing in configuration.");
        }

        public async Task<FullSteamPlayerInfo?> GetFullSteamPlayerInfoAsync(string steamId64)
        {
            if (_cache.TryGetValue(steamId64, out FullSteamPlayerInfo? cached))
                return cached;

            try
            {
                var profileUrl = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={_apiKey}&steamids={steamId64}";
                var profileResponse = await _httpClient.GetFromJsonAsync<SteamApiResponse>(profileUrl);
                var player = profileResponse?.Response?.Players?.FirstOrDefault();


                var bansUrl = $"https://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key={_apiKey}&steamids={steamId64}";
                var banResponse = await _httpClient.GetFromJsonAsync<SteamBanResponse>(bansUrl);
                var banInfo = banResponse?.Players?.FirstOrDefault();

                var fullInfo = new FullSteamPlayerInfo
                {
                    PlayerInfo = player,
                    BanInfo = banInfo
                };

                if(player is not null && banInfo is not null)
                {
                    _cache.Set(steamId64, fullInfo, TimeSpan.FromMinutes(30));
                }

                return fullInfo;
            }
            catch (HttpRequestException ex) when ((ex.StatusCode ?? 0) == HttpStatusCode.TooManyRequests)
            {
                _logger.LogError(ex, "Steam API rate limit exceeded.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении информации о Steam-профиле: {SteamId}", steamId64);
                return null;
            }
        }
    }
}
