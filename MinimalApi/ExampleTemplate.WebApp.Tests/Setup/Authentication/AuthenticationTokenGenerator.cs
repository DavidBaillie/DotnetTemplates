using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExampleTemplate.WebApp.Tests.Setup.Authentication;

/// <summary>
/// Class handles generating and parsing JWT's for use during the test project's runtime.
/// </summary>
public static class AuthenticationTokenGenerator
{
    /// <summary>
    /// Placeholder name for the scheme
    /// </summary>
    private const string authenticationScheme = "Test-Authentication";

    /// <summary>
    /// Secret key used to sign the token
    /// </summary>
    private const string secretKey = "a355e16e-331c-49d4-b24f-bb4e442aad5a";

    public const string issuer = "someIssuer";
    public const string audience = "someAudience";

    public static string GenerateJwtToken(IDictionary<string, string> claims)
    {
        // Create security key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        // Create token
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: [.. claims.Select(x => new Claim(x.Key, x.Value))],
            expires: DateTime.Now.AddMinutes(30), // Token expiration
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        // Return the token as a string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static AuthenticationTicket ValidateIssuedToken(this string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler() { MapInboundClaims = false };
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };

        var principal = tokenHandler.ValidateToken(
            token,
            validationParameters,
            out var _);

        return new AuthenticationTicket(principal, authenticationScheme);
    }
}
