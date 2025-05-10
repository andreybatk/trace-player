using TracePlayer.BL.Services.Auth;
using Microsoft.Extensions.DependencyInjection;
using TracePlayer.BL.Services.Steam;
using TracePlayer.BL.Services.Geo;
using TracePlayer.BL.Services.Fungun;

namespace TracePlayer.BL.DiContainer
{
    public static class DiServicesExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<AuthenticatorService>();
            services.AddScoped<AccessTokenGeneratorService>();

            services.AddSingleton<RefreshTokenGeneratorService>();
            services.AddSingleton<TokenGeneratorService>();
            services.AddSingleton<GeoService>();
            services.AddSingleton<SteamApiService>();
            services.AddSingleton<FungunApiService>();
        }
    }
}