using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TracePlayer.Contracts.Fungun;

namespace TracePlayer.BL.Services.Fungun
{
    public class FungunApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<FungunApiService> _logger;

        public FungunApiService(HttpClient httpClient, IMemoryCache cache, ILogger<FungunApiService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<FungunPlayerResult>?> GetFungunEntriesBySteamIdAsync(string steamId)
        {
            if (_cache.TryGetValue(steamId, out List<FungunPlayerResult>? cachedResults))
                return cachedResults;

            try
            {
                //TODO: переписать под api key
                var url = $"https://fungun.net/ecd/search[value]={steamId}&search[regex]=false";

                var response = await _httpClient.GetStringAsync(url);

                if (response.Contains("\"error\":\"Access Denied\""))
                {
                    _logger.LogWarning("Access Denied from Fungun API for Steam ID: {SteamId}", steamId);
                    return null;
                }

                var result = JsonSerializer.Deserialize<FungunApiResponse>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Data != null)
                {
                    _cache.Set(steamId, result.Data, TimeSpan.FromMinutes(30));
                    return result.Data.Select(d => new FungunPlayerResult
                    {
                        Nick = d.Nick,
                        UserIp = d.User_Ip,
                        Hostname = d.Hostname,
                        ResultStatus = d.Result_Status,
                        Before = d.Before,
                        Time = d.Time,
                        MoreLink = d.More
                    }).ToList();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching data from Fungun API for Steam ID: {SteamId}", steamId);
                return null;
            }
        }
    }
}