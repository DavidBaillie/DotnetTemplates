using System.ComponentModel.DataAnnotations;

namespace ExampleTemplate.Domain.Models.Options;

/// <summary>
/// Configuration settings for JWT authentication and authorization.
/// </summary>
public sealed class AuthenticationSettings
{
    /// <summary>
    /// The well-known endpoint URL for OpenID Connect discovery.
    /// </summary>
    [Required]
    public string WellKnownEndpoint { get; set; } = default!;

    /// <summary>
    /// The expected issuer of the JWT tokens.
    /// </summary>
    [Required]
    public string Issuer { get; set; } = default!;
}
