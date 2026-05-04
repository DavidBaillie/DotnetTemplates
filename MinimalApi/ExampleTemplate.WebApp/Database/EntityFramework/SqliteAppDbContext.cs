using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExampleTemplate.WebApp.Database.EntityFramework;

/// <summary>
/// Default provider that is included with all projects. Allows developers to work locally with a database without needing 
/// any Migrations to be defined. Place testing and local developer data here for easy access.
/// </summary>
public sealed class SqliteAppDbContext : AppDbContext
{
    /// <summary>
    /// Special object used with the sqlite runtime to make sure at least one connection remains open to the in-memory 
    /// database. If all connections close the runtime will delete the database until a new connection opens. 
    /// 
    /// When this is registered as a singleton it allows the connection to remain open for the duration of the debug run.
    /// </summary>
    public sealed class ConnectionPersistor : IDisposable
    {
        private readonly SqliteConnection connection;

        public ConnectionPersistor()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
        }
    }

    public SqliteAppDbContext(DbContextOptions<SqliteAppDbContext> options, ConnectionPersistor persistor) : base(options)
    {
        // Need to make sure the file system has the database entity for usage
        Database.EnsureCreated();
        _ = persistor;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
