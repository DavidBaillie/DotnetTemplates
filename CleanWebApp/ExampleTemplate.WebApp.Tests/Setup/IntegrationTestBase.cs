using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ExampleTemplate.Infrastructure.Database.EntityFramework;

namespace ExampleTemplate.WebApp.Tests.Setup;

/// <summary>
/// Base class available for all tests to configure some defaults when working with the <see cref="CustomWebApplicationFactory"/>
/// </summary>
[Parallelizable(ParallelScope.All)]
[TestFixture, Category("Integration")]
public abstract class IntegrationTestBase : IDisposable
{
    private CustomWebApplicationFactory? _factory;
    private HttpClient? _client;
    private IServiceScope? _scope;
    private IDbContextFactory<AppDbContext>? _dbContextFactory;

    /// <summary>
    /// Factory for accessing members and functions
    /// </summary>
    protected CustomWebApplicationFactory Factory => _factory ??= new CustomWebApplicationFactory();

    /// <summary>
    /// Pregenerated HttpClient for making API calls against the endpoints.
    /// </summary>
    protected HttpClient Client => _client ??= Factory.CreateClient();

    /// <summary>
    /// Service Scope for the webapp runtime
    /// </summary>
    protected IServiceScope Scope => _scope ??= Factory.Services.CreateScope();

    /// <summary>
    /// DbContext factory for accessing <see cref="AppDbContext"/> instances during tests
    /// </summary>
    protected IDbContextFactory<AppDbContext> DbContextFactory => _dbContextFactory ??= Scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();

    public virtual void Dispose()
    {
        _scope?.Dispose();
        GC.SuppressFinalize(this);
    }
}
