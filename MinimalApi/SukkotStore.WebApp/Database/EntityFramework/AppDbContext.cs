using Microsoft.EntityFrameworkCore;
using SukkotStore.WebApp.Constants;
using SukkotStore.WebApp.Database.Models;

namespace SukkotStore.WebApp.Database.EntityFramework;

public abstract class AppDbContext(DbContextOptions options) : DbContext(options)
{
    private static readonly string SCHEMA_NAME = "sukkotstore";

    public DbSet<UserEntity> Users { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SCHEMA_NAME);

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.DisplayName).HasMaxLength(256);
            entity.ToTable("InternalUsers");
        });
    }
}
