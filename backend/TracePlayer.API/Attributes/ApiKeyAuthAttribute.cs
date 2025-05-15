using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using TracePlayer.BL.Services.Api;

namespace TracePlayer.API.Attributes
{
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "X-API-Key";

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

            await next();
        }
    }
}
