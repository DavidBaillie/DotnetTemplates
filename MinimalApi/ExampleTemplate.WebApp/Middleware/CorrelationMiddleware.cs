namespace ExampleTemplate.WebApp.Middleware;

/// <summary>
/// This middleware handles generating a correlation id that can be accessed and referenced in logs when a customer makes an API 
/// call to the server. This way the customer can provide the response's Correlation Id header value and developers can search logs 
/// for that Id.
/// </summary>
public sealed class CorrelationMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Response header which will contain the correlation id used by the system 
    /// </summary>
    public const string CORRELATION_HEADER_KEY = "X-Correlation-Id";

    /// <summary>
    /// Key for the logger to track the correlation id in all logs made after this middleware runs
    /// </summary>
    public const string CORRELATION_LOGGING_KEY = "Client-Correlation-Id";

    public async Task Invoke(
        HttpContext httpContext,
        ILogger<CorrelationMiddleware> logger)
    {
        var requestId = Guid.NewGuid().ToString();
        using (logger.BeginScope(new Dictionary<string, object>() { [CORRELATION_LOGGING_KEY] = requestId }))
        {
            httpContext.Response.Headers.Append(CORRELATION_HEADER_KEY, requestId);
            await next(httpContext);
        }
    }
}