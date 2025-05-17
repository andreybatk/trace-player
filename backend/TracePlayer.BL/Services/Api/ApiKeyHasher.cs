using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace TracePlayer.BL.Services.Api
{
    public class ApiKeyHasher
    {
        private readonly string _secret;

        public ApiKeyHasher(IConfiguration configuration)
        {
            _secret = configuration["Api:Secret"] ?? throw new InvalidOperationException("Secret for ApiKeyHasher is missing in configuration.");
        }

        public string ComputeHash(string apiKey)
        {
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secret));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(apiKey));
            return Convert.ToBase64String(hashBytes);
        }
    }
}