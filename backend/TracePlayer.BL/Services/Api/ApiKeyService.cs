using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using TracePlayer.BL.Services.Steam;
using TracePlayer.DB.Repositories.Api;

namespace TracePlayer.BL.Services.Api
{
    public class ApiKeyService
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly ApiKeyHasher _apiKeyHasher;
        private readonly ILogger<SteamApiService> _logger;

        public ApiKeyService(IApiKeyRepository apiKeyRepository, ApiKeyHasher apiKeyHasher, ILogger<SteamApiService> logger)
        {
            _apiKeyRepository = apiKeyRepository;
            _apiKeyHasher = apiKeyHasher;
            _logger = logger;
        }

        public async Task<string?> GenerateAndSaveApiKey(string serverIp)
        {
            var rawKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var keyHash = _apiKeyHasher.ComputeHash(rawKey);

            var result = await _apiKeyRepository.AddKeyHash(serverIp, keyHash);

            if(!result)
            {
                return null;
            }

            return rawKey;
        }

        public async Task<bool> IsValidApiKey(string apiKey)
        {
            var apiKeyHash = _apiKeyHasher.ComputeHash(apiKey);
            return await _apiKeyRepository.IsValidKeyHash(apiKeyHash);
        }

        public async Task<bool> IsValidServerIp(string serverIp)
        {
            return await _apiKeyRepository.IsValidServerIp(serverIp);
        }

        public async Task<List<string>> GetAllServerIps()
        {
            return await _apiKeyRepository.GetAllServerIps();
        }

        public async Task<bool> DeleteApiKeyByServerIp(string serverIp)
        {
            return await _apiKeyRepository.DeleteApiKeyByServerIp(serverIp);
        }
    }
}