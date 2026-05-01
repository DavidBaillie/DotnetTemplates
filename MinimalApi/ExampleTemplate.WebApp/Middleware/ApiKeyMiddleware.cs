using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using SukkotStore.WebApp.Models.Options;

namespace SukkotStore.WebApp.Middleware;

public class ApiKeyMiddleware(RequestDelegate next)
{
    private const string API_KEY_NAME = "x-api-key";

    public async Task Invoke(
        HttpContext httpContext,
        IOptions<AuthenticationSettings> authSettings,
        ILogger<ApiKeyMiddleware> logger)
    {
        // Check the API key is the expected value
        if (!httpContext.Request.Headers.TryGetValue(API_KEY_NAME, out var value) ||
            StringValues.IsNullOrEmpty(value) ||
            !value.ToString().Equals(authSettings.Value.ApiKey, StringComparison.Ordinal))
        {
            logger.LogDebug("Rejected a request without the required API Key!");

            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await next(httpContext);
    }
}

