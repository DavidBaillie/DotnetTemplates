using System.ComponentModel.DataAnnotations;

namespace SukkotStore.WebApp.Models.Requests;

public sealed class CreateUserRequest
{
    [Required, Length(3, 64)]
    public required string Name { get; set; }

    public string[] Roles { get; set; } = [];
}
