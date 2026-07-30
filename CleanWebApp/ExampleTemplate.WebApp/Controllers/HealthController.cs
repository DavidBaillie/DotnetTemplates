using Microsoft.AspNetCore.Mvc;

namespace ExampleTemplate.WebApp.Controllers;

/// <summary>
/// Provides health check endpoints for monitoring application availability.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class HealthController : ControllerBase
{
    /// <summary>
    /// Returns a simple health status indicating the application is running.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>An HTTP 200 OK response if the application is healthy.</returns>
    [HttpGet]
    public Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IActionResult>(Ok());
    }
}
