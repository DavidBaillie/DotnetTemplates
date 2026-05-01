using System.ComponentModel.DataAnnotations;

namespace SukkotStore.WebApp.Models.Options;

public sealed class AuthenticationSettings
{
    [Required]
    public string ApiKey { get; set; } = default!;

    public string AuthenticationServerEndpoint { get; set; } = default!;
    public string WellKnownEndpoint { get; set; } = default!;
    public string Issuer { get; set; } = default!;
}
