namespace SukkotStore.WebApp.Database.Models;

public class UserEntity
{
    public required Guid Id { get; set; }
    public required string DisplayName { get; set; }
    public required string[] Roles { get; set; } = [];
}
