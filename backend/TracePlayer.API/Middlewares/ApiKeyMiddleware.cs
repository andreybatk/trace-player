using TracePlayer.BL.Services.Api;

namespace TracePlayer.API.Middlewares
{
    public class ApiKeyMiddleware
    {
        private const string ApiKeyHeaderName = "X-API-Key";
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApiKeyService apiKeyService)
        {
            var path = context.Request.Path;

            if (path.StartsWithSegments("/api/auth/steam-login") || path.StartsWithSegments("/api/auth/steam-callback"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API Key is missing.");
                return;
            }

            var isValid = await apiKeyService.IsValidApiKey(extractedApiKey!);
            if (!isValid)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid API Key.");
                return;
            }

            await _next(context);
        }
    }
}