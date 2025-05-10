using Microsoft.AspNetCore.Mvc.ModelBinding;
using TracePlayer.API.Validators;
using TracePlayer.BL.Services.Geo;

namespace TracePlayer.API.DiContainer
{
    public static class Extensions
    {
        public static void AddValidators(this IServiceCollection services)
        {
            services.AddSingleton<RefreshTokenValidator>();
        }
    }
}