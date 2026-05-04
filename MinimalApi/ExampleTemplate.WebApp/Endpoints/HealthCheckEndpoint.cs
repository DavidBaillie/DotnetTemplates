namespace ExampleTemplate.WebApp.Endpoints;

public sealed class HealthCheckEndpoint
{
    public static Task<IResult> GetAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult((IResult)TypedResults.Ok());
    }
}