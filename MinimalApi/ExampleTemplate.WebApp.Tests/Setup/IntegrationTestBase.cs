using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ExampleTemplate.WebApp.Database.EntityFramework;

namespace ExampleTemplate.WebApp.Tests.Setup;

[Parallelizable(ParallelScope.All)]
[TestFixture, Category("Integration")]
public abstract class IntegrationTestBase : IDisposable
{
    protected readonly StoreWebApplicationFactory Factory;
    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly IDbContextFactory<AppDbContext> DbContextFactory;

    public IntegrationTestBase()
    {
        Factory = new StoreWebApplicationFactory();
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
