using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ExampleTemplate.WebApp.Database.EntityFramework;

namespace ExampleTemplate.WebApp.Tests.Setup;

/// <summary>
/// Base class available for all tests to configure some defaults when working with the <see cref="CustomWebApplicationFactory"/>
/// </summary>
[Parallelizable(ParallelScope.All)]
[TestFixture, Category("Integration")]
public abstract class IntegrationTestBase : IDisposable
{
    /// <summary>
    /// Factory for accessing members and functions
    /// </summary>
    protected readonly CustomWebApplicationFactory Factory;

    /// <summary>
    /// Pregenerated HttpClient for making API calls against the endpoints.
    /// </summary>
    protected readonly HttpClient Client;

    /// <summary>
    /// Service Scope for the webapp runtime
    /// </summary>
    protected readonly IServiceScope Scope;

    /// <summary>
    /// DbContext factory for accessing <see cref="AppDbContext"/> instances during tests
    /// </summary>
    protected readonly IDbContextFactory<AppDbContext> DbContextFactory;

    public IntegrationTestBase()
    {
        Factory = new CustomWebApplicationFactory();
        Client = Factory.CreateClient();

        Scope = Factory.Services.CreateScope();
        DbContextFactory = Scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    }

    public virtual void Dispose()
    {
        Scope.Dispose();
        GC.SuppressFinalize(this);
    }
}
