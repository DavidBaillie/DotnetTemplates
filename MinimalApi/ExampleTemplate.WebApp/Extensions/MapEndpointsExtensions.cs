using ExampleTemplate.WebApp.Endpoints;

namespace ExampleTemplate.WebApp.Extensions;

public static class MapEndpointsExtensions
{
    public static IEndpointRouteBuilder RegisterEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/health", HealthCheckEndpoint.GetAsync)
            .AllowAnonymous();

        return endpoints;
    }
}
