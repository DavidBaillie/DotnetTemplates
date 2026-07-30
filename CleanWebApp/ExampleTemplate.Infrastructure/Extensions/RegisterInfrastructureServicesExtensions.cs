using ExampleTemplate.Domain.Models.Options;
using ExampleTemplate.Infrastructure.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ExampleTemplate.Infrastructure.Extensions;

public static class RegisterInfrastructureServicesExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, DatabaseSettings databaseOptions)
    {
        services.RegisterDatabaseContext(databaseOptions);

        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>(
                name: "database",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["db", "database"]);

        return services;
    }
}