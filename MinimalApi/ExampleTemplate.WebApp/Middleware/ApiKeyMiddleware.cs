using Microsoft.Extensions.Primitives;

namespace ExampleTemplate.WebApp.Middleware;

/// <summary>
/// Middleware responsible for checking that all requests contain the required API Key for endpoints. 
/// When the key is missing, execution is halted and the user received an Unauthorized response from the 
/// server.
/// </summary>
public class ApiKeyMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Key found in the request header to use when looking for the API Key
    /// </summary>
    private const string API_KEY_HEADER_NAME = "x-api-key";

    /// <summary>
    /// Key found in <see cref="IConfiguration"/> to lookup the expected Api Key value 
    /// </summary>
    private const string API_KEY_CONFIGURATION_NAME = "ApiKey";

    public async Task Invoke(
        HttpContext httpContext,
        IConfiguration configuration)
    {
        // Grab the developer provided API key from configuration
        var apiKey = configuration[API_KEY_CONFIGURATION_NAME] ??
            throw new NullReferenceException($"Failed to find the required [{API_KEY_CONFIGURATION_NAME}] key in configuration.");

        // Check the inbound request contains the expected value
        if (!httpContext.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var value) ||
            StringValues.IsNullOrEmpty(value) ||
            !value.ToString().Equals(apiKey, StringComparison.Ordinal))
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await next(httpContext);
    }
}

