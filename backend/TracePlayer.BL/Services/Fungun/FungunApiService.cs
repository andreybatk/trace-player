using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using TracePlayer.Contracts.Fungun;

namespace TracePlayer.BL.Services.Fungun
{
    public class FungunApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<FungunApiService> _logger;
        private readonly string _apiKeyMd5;

        public FungunApiService(HttpClient httpClient, IMemoryCache cache, ILogger<FungunApiService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
            _apiKeyMd5 = configuration["Fungun:ApiKeyMd5"] ?? throw new InvalidOperationException("Fungun API key is missing in configuration.");
        }

        public async Task<FungunPlayer?> GetFungunEntriesBySteamIdAsync(string steamId, string? fungunApiKeyMd5, CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue(steamId, out FungunPlayer? cachedResults))
                return cachedResults;

            try
            {
                if (fungunApiKeyMd5 is null)
                {
                    fungunApiKeyMd5 = _apiKeyMd5;
                }

                var url = $"http://fungun.net/ska/eye.php?method=get&key={fungunApiKeyMd5}&steamids[]={steamId}";

                var fungunResponse = await _httpClient.GetFromJsonAsync<FungunApiResponse>(url, cancellationToken);

                if (fungunResponse?.Success == true && fungunResponse.Data.TryGetValue(steamId, out var results))
                {
                    var lastSuccess = results.LastOrDefault(r => r.ResultStatus == "success");
                    var lastWarning = results.LastOrDefault(r => r.ResultStatus == "warning");
                    var lastDanger = results.LastOrDefault(r => r.ResultStatus == "danger");
                    var lastReport = results.LastOrDefault();
                    var fungunPlayer = new FungunPlayer { LastSuccess = lastSuccess, LastWarning = lastWarning, LastDanger = lastDanger, LastReport = lastReport };

                    _cache.Set(steamId, fungunPlayer, TimeSpan.FromMinutes(5));

                    return fungunPlayer;
                }

                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Fungun API request was canceled");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error fetching data from Fungun API for Steam ID: {SteamId}", steamId);
                return null;
            }
        }

        private static string CreateMD5(string input)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}