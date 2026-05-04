using ExampleTemplate.WebApp.Database.EntityFramework.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ExampleTemplate.WebApp.Database.EntityFramework;
using ExampleTemplate.WebApp.Models.Options;

namespace ExampleTemplate.WebApp.Extensions;

public static class RegisterDbContextExtensions
{
    /// <summary>
    /// Configuration value specifying that the local in-memory database should be used
    /// </summary>
    private const string MEMORY_PROVIDER = "sqlite";

    //#if (usePostgreSql)
    /// <summary>
    /// Configuration value specifying that a postgresql database should be used
    /// </summary>
    private const string POSTGRESQL_PROVIDER = "postgresql";
    //#endif

    /// <summary>
    /// Connection string to the local in-memory database. This can be changed in the case when multiple runtimes 
    /// for the AppDbContext are present in the same process. 
    /// </summary>
    private static readonly string MEMORY_CONNECTION_STRING = "DataSource=file::memory:?cache=shared";

    /// <summary>
    /// Registered the database context for the application to use as a data source for the runtime.
    /// </summary>
    /// <param name="services">Link to service container</param>
    /// <param name="databaseOptions">Options for configuring the database</param>
    /// <returns>Cascading reference to service container</returns>
    public static IServiceCollection RegisterDatabaseContext(this IServiceCollection services, DatabaseSettings databaseOptions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseOptions.Provider, nameof(databaseOptions.Provider));

        // Select the correct database provider for Entity Framework 
        switch (databaseOptions.Provider.ToLowerInvariant())
        {
            //#if (usePostgreSql)
            case POSTGRESQL_PROVIDER:
                ArgumentException.ThrowIfNullOrWhiteSpace(databaseOptions.ConnectionString, nameof(databaseOptions.ConnectionString));

                services.TryAddScoped<IDbContextFactory<AppDbContext>, PooledAppDbContextFactory<PostgresAppDbContext>>();
                services.AddPooledDbContextFactory<PostgresAppDbContext>(options =>
                {
                    options.UseNpgsql(databaseOptions.ConnectionString);
                });
                break;
            //#endif    
            case MEMORY_PROVIDER:
                services.TryAddSingleton<SqliteAppDbContext.ConnectionPersistor>();
                services.TryAddScoped<IDbContextFactory<AppDbContext>, PooledAppDbContextFactory<SqliteAppDbContext>>();
                services.AddPooledDbContextFactory<SqliteAppDbContext>(options =>
                {
                    options.UseSqlite(MEMORY_CONNECTION_STRING);
                });
                break;
            default:
                throw new ApplicationException($"Failed to find a valid database provider for the application to use. Provided value: [{databaseOptions.Provider}]");
        }

        return services;
    }
}
