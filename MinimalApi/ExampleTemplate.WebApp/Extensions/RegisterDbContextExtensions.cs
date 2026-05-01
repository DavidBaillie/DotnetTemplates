using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SukkotStore.WebApp.Database.EntityFramework;
using SukkotStore.WebApp.Models.Options;

namespace SukkotStore.WebApp.Extensions;

public static class RegisterDbContextExtensions
{
    /// <summary>
    /// Configuration value specifying that the local in-memory database should be used
    /// </summary>
    private static readonly string MEMORY_PROVIDER = "sqlite";

    /// <summary>
    /// Connection string to the local in-memory database
    /// </summary>
    private static readonly string MEMORY_CONNECTION_STRING = "DataSource=file::memory:?cache=shared";

    /// <summary>
    /// Custom migrations table name to be used in deployments. 
    /// This allows Entity Framework to share a database without stepping over itself when multiple 
    /// deployments are present in the same database. 
    /// 
    /// Namely, I'm cheap and don't want a bunch of databases.
    /// </summary>
    private static readonly string MIGRATION_TABLE_NAME = "__sukkotstore_migrations";

    /// <summary>
    /// Registered the database context for the application to use as a data source for the runtime.
    /// </summary>
    /// <param name="services">Link to service container</param>
    /// <param name="databaseOptions">Options for configuring the database</param>
    /// <returns>Cascading reference to service container</returns>
    public static IServiceCollection RegisterDatabaseContext(this IServiceCollection services, DatabaseSettings databaseOptions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseOptions.Provider, nameof(databaseOptions.Provider));

        // Local development usage, loads the memory database for faster startup and teardown of the database between runs
        if (databaseOptions.Provider.Equals(MEMORY_PROVIDER, StringComparison.InvariantCultureIgnoreCase))
        {
            services.TryAddSingleton<SqliteAppDbContext.ConnectionPersistor>();
            services.TryAddScoped<IDbContextFactory<AppDbContext>, PooledAppDbContextFactory<SqliteAppDbContext>>();
            services.AddPooledDbContextFactory<SqliteAppDbContext>(options =>
            {
                options.UseSqlite(MEMORY_CONNECTION_STRING);
            });
        }
        // Default implementation for deployments and testing
        else
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(databaseOptions.ConnectionString, nameof(databaseOptions.ConnectionString));

            services.TryAddScoped<IDbContextFactory<AppDbContext>, PooledAppDbContextFactory<PostgresAppDbContext>>();
            services.AddPooledDbContextFactory<PostgresAppDbContext>(options =>
            {
                options.UseNpgsql(databaseOptions.ConnectionString,
                    x => x.MigrationsHistoryTable(MIGRATION_TABLE_NAME));
            });
        }
        return services;
    }
}
