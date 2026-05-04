using ExampleTemplate.WebApp.Database.EntityFramework.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace ExampleTemplate.WebApp.Tests.Integration;

/// <summary>
/// Sets up tests for the current folder (and sub-folders). Any dependencies that need to exist 
/// before the tests begin should be stored here.
/// </summary>
[SetUpFixture]
public sealed class IntegrationTestLifeCycle
{
    //#if (usePostgreSql && testWithPostgreSql)
    private readonly PostgreSqlContainer psqlContainer = new PostgreSqlBuilder("postgres:18-alpine").Build();
    //#endif

    [OneTimeSetUp]
    public async Task SetupAsync()
    {
        //#if (usePostgreSql && testWithPostgreSql)
        // Spin up a postgres database 
        await psqlContainer.StartAsync();
        var connectionString = psqlContainer.GetConnectionString();

        var contextOptions = new DbContextOptionsBuilder<PostgresAppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        Environment.SetEnvironmentVariable("Database__ConnectionString", connectionString + "; Include Error Detail=true");
        Environment.SetEnvironmentVariable("Database__Provider", "postgres");

        using var dbContext = new PostgresAppDbContext(contextOptions);
        await dbContext.Database.MigrateAsync();
        //#endif

        TestContext.Progress.WriteLine($"Running Integration Tests\n");
    }

    [OneTimeTearDown]
    public async Task TearDownAsync()
    {
        //#if (usePostgreSql && testWithPostgreSql)
        await psqlContainer.DisposeAsync();
        //#endif

        TestContext.Progress.WriteLine($"Integration Tests Completed, cleaning up resources");
    }
}
