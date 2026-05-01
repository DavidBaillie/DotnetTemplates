using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace SukkotStore.WebApp.Database.EntityFramework;

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
        Database.EnsureCreated();
        _ = persistor;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
