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

        public async Task<SteamPlayerInfo?> GetPlayerInfoAsync(string steamId64)
        {
            if (_cache.TryGetValue(steamId64, out SteamPlayerInfo? cachedPlayer))
                return cachedPlayer;

            try
            {
                var url = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={_apiKey}&steamids={steamId64}";
                var response = await _httpClient.GetFromJsonAsync<SteamApiResponse>(url);

                var player = response?.Response?.Players?.FirstOrDefault();

                if (player != null)
                {
                    _cache.Set(steamId64, player, TimeSpan.FromMinutes(30));
                }

                return player;
            }
            catch (HttpRequestException ex) when ((ex.StatusCode ?? 0) == HttpStatusCode.TooManyRequests)
            {
                _logger.LogError(ex, "Steam API rate limit exceeded.");
                return null;
            }
        }
    }
}
