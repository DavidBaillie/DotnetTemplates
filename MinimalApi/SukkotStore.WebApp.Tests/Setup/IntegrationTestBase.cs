using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SukkotStore.WebApp.Database.EntityFramework;

namespace SukkotStore.WebApp.Tests.Setup;

[Parallelizable(ParallelScope.All)]
[TestFixture, Category("Integration")]
public abstract class IntegrationTestBase : IDisposable
{
    public static readonly Guid ADMIN_USER_ID = Guid.Parse("78e788d4-b82e-4b8a-9bdf-cd6f160a7625");
    public static readonly Guid COMMON_USER_ID = Guid.Parse("48ae770f-395f-4988-982a-c713a3f67f16");

    protected readonly StoreWebApplicationFactory Factory;
    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly IDbContextFactory<AppDbContext> DbContextFactory;

    public IntegrationTestBase(TestUserContext? context)
    {
        Factory = new StoreWebApplicationFactory(context);
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
