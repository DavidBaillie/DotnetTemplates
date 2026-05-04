using Microsoft.EntityFrameworkCore;
using ExampleTemplate.WebApp.Database.Models;

namespace ExampleTemplate.WebApp.Database.EntityFramework;

/// <summary>
/// Base class used for the <see cref="DbContext"/>. When any provider for the <see cref="DbContext"/> is being used across the 
/// project, this class should be the concrete implementation referenced. Provider implementations should inherit from this, 
/// be injected into the DI Container, and then referenced as this base class for usage. 
/// This is important so as to allow providers to be changed at runtime based on developer settings.
/// </summary>
public abstract class AppDbContext(DbContextOptions options) : DbContext(options)
{
    // Your DbSet<T> here

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
