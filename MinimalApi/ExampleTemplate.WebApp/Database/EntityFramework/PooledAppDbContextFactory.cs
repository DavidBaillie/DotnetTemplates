using Microsoft.EntityFrameworkCore;

namespace ExampleTemplate.WebApp.Database.EntityFramework;

/// <summary>
/// Defines a DbContext factory for accessing pooled instances of the <see cref="AppDbContext"/> from the generic base. 
/// This is important as it allows the provider for the <see cref="AppDbContext"/> to be registered under the concrete 
/// type and then converted into the base class <see cref="AppDbContext"/> for wide-spread usage across the project.
/// </summary>
/// <typeparam name="T">Provider for the <see cref="AppDbContext"/></typeparam>
/// <param name="factory">Implementation used by the pooled DbContext library at runtime</param>
public class PooledAppDbContextFactory<T>(IDbContextFactory<T> factory)
    : IDbContextFactory<AppDbContext>
    where T : AppDbContext
{
    // NOTE: This handles converting the DbContext Provider from the registered type in the DI container into 
    //       the base AppDbContext that will be used throughout the project.
    public AppDbContext CreateDbContext()
        => factory.CreateDbContext();
}
