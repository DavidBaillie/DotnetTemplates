using System.ComponentModel.DataAnnotations;

namespace SukkotStore.WebApp.Models.Options;

public sealed class DatabaseSettings
{
    [Required]
    public string Provider { get; set; } = default!;

    public string ConnectionString { get; set; } = default!;
}
