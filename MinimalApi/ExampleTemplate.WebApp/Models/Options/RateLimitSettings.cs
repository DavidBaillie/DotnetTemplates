using System.ComponentModel.DataAnnotations;

namespace ExampleTemplate.WebApp.Models.Options;

/// <summary>
/// Configuration settings for API rate limiting.
/// </summary>
public sealed class RateLimitSettings
{
    /// <summary>
    /// Whether rate limiting is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Number of requests allowed per time window.
    /// </summary>
    [Range(1, 10000)]
    public int PermitLimit { get; set; } = 100;

    /// <summary>
    /// Time window in seconds for the rate limit.
    /// </summary>
    [Range(1, 3600)]
    public int WindowSeconds { get; set; } = 60;

    /// <summary>
    /// Number of requests that can queue when limit is reached.
    /// </summary>
    [Range(0, 100)]
    public int QueueLimit { get; set; } = 0;
}
