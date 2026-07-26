using ExampleTemplate.WebApp.Endpoints;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace ExampleTemplate.WebApp.Extensions;

/// <summary>
/// Extension methods for registering Minimal API endpoints.
/// </summary>
public static class MapEndpointsExtensions
{
    /// <summary>
    /// Registers all health check and API endpoints for the application.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to add routes to.</param>
    /// <returns>The endpoint route builder for method chaining.</returns>
    public static IEndpointRouteBuilder RegisterEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Basic health check endpoint - returns 200 OK if healthy
        endpoints.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        duration = e.Value.Duration.TotalMilliseconds
                    }),
                    totalDuration = report.TotalDuration.TotalMilliseconds
                });
                await context.Response.WriteAsync(result);
            }
        }).AllowAnonymous();

        // Liveness probe - simple check without dependencies
        endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false // No checks, just returns 200 if app is running
        }).AllowAnonymous();

        // Readiness probe - includes all checks
        endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => true // All checks
        }).AllowAnonymous();

        return endpoints;
    }
}
