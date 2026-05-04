using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ExampleTemplate.WebApp.Middleware;

/// <summary>
/// General fallback that catches all exceptions and returns an Internal Server Error.
/// </summary>
public sealed class FallbackMiddleware(
    RequestDelegate next,
    ILogger<FallbackMiddleware> logger)
{
    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fallback middleware caught an exception while processing a request");
            var responseObject = new InternalServerErrorProblemDetails();

#if DEBUG
            // When in debug builds provide the exception as part of the response to make debug easier.
            responseObject.Exception = ex;
#endif
            // Write the response information manually into the httpContext
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.Headers.Append("Content-Type", "application/problem+json; charset=utf-8");
            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(responseObject));
        }
    }

    public class InternalServerErrorProblemDetails : ProblemDetails
    {
        public Exception? Exception { get; set; }
    }
}
