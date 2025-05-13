using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using TracePlayer.BL.Services.Api;

namespace TracePlayer.API
{
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "X-API-Key";
        private const string ForwardedForHeader = "X-Forwarded-For";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var apiKeyService = context.HttpContext.RequestServices.GetRequiredService<ApiKeyService>();

            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("API Key is missing.");
                return;
            }

            var isValidKey = await apiKeyService.IsValidApiKey(extractedApiKey!);
            if (!isValidKey)
            {
                context.Result = new UnauthorizedObjectResult("Invalid API Key.");
                return;
            }

            var ip = GetClientIp(context.HttpContext);
            var isValidIp = await apiKeyService.IsValidServerIp(ip);

            if (!isValidIp)
            {
                context.Result = new UnauthorizedObjectResult("Invalid Server IP.");
                return;
            }

            await next();
        }

        private string GetClientIp(HttpContext httpContext)
        {
            if (httpContext.Request.Headers.TryGetValue(ForwardedForHeader, out var forwardedFor))
            {
                var ipList = forwardedFor.ToString().Split(',');
                if (ipList.Length > 0)
                {
                    return ipList[0].Trim();
                }
            }

            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}
