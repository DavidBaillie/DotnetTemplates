using Microsoft.EntityFrameworkCore;
using SukkotStore.WebApp.Constants;
using SukkotStore.WebApp.Database.EntityFramework;
using SukkotStore.WebApp.Database.Models;
using SukkotStore.WebApp.Tests.Setup;
using Testcontainers.PostgreSql;

namespace SukkotStore.WebApp.Tests.Integration;

[SetUpFixture]
public sealed class IntegrationTestLifeCycle
{
    private readonly PostgreSqlContainer psqlContainer = new PostgreSqlBuilder("postgres:18-alpine").Build();

    [OneTimeSetUp]
    public async Task SetupAsync()
    {
        await psqlContainer.StartAsync();
        var connectionString = psqlContainer.GetConnectionString();

        var contextOptions = new DbContextOptionsBuilder<PostgresAppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        Environment.SetEnvironmentVariable("Database__ConnectionString", connectionString + "; Include Error Detail=true");
        Environment.SetEnvironmentVariable("Database__Provider", "postgres");

        using var dbContext = new PostgresAppDbContext(contextOptions);
        await dbContext.Database.MigrateAsync();
        await SeedDatabaseAsync(dbContext);

        TestContext.Progress.WriteLine($"Running Store Integration Tests\n");
    }

    [OneTimeTearDown]
    public async Task TearDownAsync()
    {
        await psqlContainer.DisposeAsync();
        TestContext.Progress.WriteLine($"Integration Tests Completed, cleaning up resources");
    }

    private static async Task SeedDatabaseAsync(AppDbContext dbContext)
    {
        await dbContext.Users
            .AddRangeAsync([
                // Admin user
                new UserEntity() {
                    Id = IntegrationTestBase.ADMIN_USER_ID,
                    DisplayName = "Test User 1",
                    Roles = [UserRoleConstants.ADMIN_ROLE]
                },
                // General User
                new UserEntity() {
                    Id = IntegrationTestBase.COMMON_USER_ID,
                    DisplayName = "Test User 2",
                    Roles = []
                }
            ]);

        await dbContext.SaveChangesAsync();
    }
}
