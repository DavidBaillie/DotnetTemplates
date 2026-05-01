using Microsoft.EntityFrameworkCore;

namespace SukkotStore.WebApp.Database.EntityFramework;

public class PooledAppDbContextFactory<T>(IDbContextFactory<T> factory)
    : IDbContextFactory<AppDbContext>
    where T : AppDbContext
{
    // NOTE: This handles converting the DbContext Provider from the registered type in the DI container into 
    //       the base AppDbContext that will be used throughout the project.
    public AppDbContext CreateDbContext()
        => factory.CreateDbContext();
}
