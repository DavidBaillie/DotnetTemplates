namespace ExampleTemplate.WebApp.Endpoints;

/// <summary>
/// Provides health check endpoints for monitoring application availability.
/// </summary>
public sealed class HealthCheckEndpoint
{
    /// <summary>
    /// Returns a simple health status indicating the application is running.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>An HTTP 200 OK response if the application is healthy.</returns>
    public static Task<IResult> GetAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult((IResult)TypedResults.Ok());
    }
}