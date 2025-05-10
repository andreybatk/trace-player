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
                var url = $"https://fungun.net/ecd/ajax/ecd_front.php?method=list&draw=2&columns[0][data]=nick&columns[1][data]=user_ip&columns[2][data]=hostname&columns[3][data]=result_status&columns[4][data]=before&columns[5][data]=time&columns[6][data]=more&order[0][column]=5&order[0][dir]=desc&start=0&length=50&search[value]={steamId}&search[regex]=false";

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
                        Nick = CleanHtml(d.Nick),
                        UserIp = CleanHtml(d.User_Ip),
                        Hostname = CleanHtml(d.Hostname),
                        ResultStatus = CleanHtml(d.Result_Status),
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

        private string CleanHtml(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
        }
    }
}