using Microsoft.EntityFrameworkCore;

namespace SukkotStore.WebApp.Database.EntityFramework;

public sealed class PostgresAppDbContext(DbContextOptions<PostgresAppDbContext> options)
    : AppDbContext(options);