using ExampleTemplate.WebApp.Database.EntityFramework.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace ExampleTemplate.WebApp.Tests.IntegrationTests;

[SetUpFixture]
public sealed class PostgreSqlIntegrationTestLifeCycle
{
    private readonly PostgreSqlContainer psqlContainer = new PostgreSqlBuilder("postgres:18-alpine").Build();

    [OneTimeSetUp]
    public async Task SetupAsync()
    {
        // Spin up a postgres database 
        await psqlContainer.StartAsync();
        var connectionString = psqlContainer.GetConnectionString();

#if includeAuth
        Environment.SetEnvironmentVariable("Authentication__WellKnownEndpoint", "https://something.ca");
        Environment.SetEnvironmentVariable("Authentication__Issuer", "someone");
#endif

        Environment.SetEnvironmentVariable("Database__ConnectionString", connectionString + "; Include Error Detail=true");
        Environment.SetEnvironmentVariable("Database__Provider", "postgresql");
        Environment.SetEnvironmentVariable("ApiKey", "test-api-key-12345");

        var contextOptions = new DbContextOptionsBuilder<PostgresAppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        using var dbContext = new PostgresAppDbContext(contextOptions);
        await dbContext.Database.MigrateAsync();

        TestContext.Progress.WriteLine($"Running Integration Tests\n");
    }

    [OneTimeTearDown]
    public async Task TearDownAsync()
    {
        await psqlContainer.DisposeAsync();
        TestContext.Progress.WriteLine($"Integration Tests Completed, cleaning up resources");
    }
}
