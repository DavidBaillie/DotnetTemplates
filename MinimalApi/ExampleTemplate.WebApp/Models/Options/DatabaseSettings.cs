using System.ComponentModel.DataAnnotations;

namespace ExampleTemplate.WebApp.Models.Options;

public sealed class DatabaseSettings
{
    [Required]
    public string Provider { get; set; } = default!;

    public string ConnectionString { get; set; } = default!;
}
