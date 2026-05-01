using SukkotStore.WebApp.Database.Models;
using SukkotStore.WebApp.Models;
using SukkotStore.WebApp.Models.Requests;

namespace SukkotStore.WebApp.Database.Mapping;

public static class UserMapper
{
    public static UserDto ToDto(this UserEntity entity) =>
        new()
        {
            Id = entity.Id,
            Name = entity.DisplayName,
        };

    public static UserEntity ToEntity(this CreateUserRequest request) =>
        new()
        {
            Id = Guid.Empty,
            DisplayName = request.Name,
            Roles = request.Roles,
        };
}
