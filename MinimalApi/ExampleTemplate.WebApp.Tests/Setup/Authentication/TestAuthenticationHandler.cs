using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace ExampleTemplate.WebApp.Tests.Setup.Authentication;

/// <summary>
/// Defines a custom authentication handler for the test project to inject into the API. 
/// It validates receives and validates the Bearer token, then generates an authentication ticket 
/// and service principle to be used by the downstream authentication policies.
/// </summary>
public class TestAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    /// <inheritdoc />
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Read the Bearer token from the inbound request
        var header = Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(header) || !header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(AuthenticateResult.NoResult());

        // Read the JWT only and feed it back into the validation service
        var ticket = header
            ["Bearer ".Length..]
            .Trim()
            .ValidateIssuedToken();

        // Pass the valid ticket along for later evalutation
        return Task.FromResult(AuthenticateResult.Success(
            new AuthenticationTicket(ticket.Principal, Scheme.Name)));
    }
}
