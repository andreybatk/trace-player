using TracePlayer.DB.Repositories.Token;
using Microsoft.Extensions.DependencyInjection;
using TracePlayer.DB.Repositories.Players;
using TracePlayer.DB.Repositories.Users;
using TracePlayer.DB.Repositories.Api;

namespace TracePlayer.DB.DiContainer
{
    public static class DiRepositoryExtensions
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
        }
    }
}