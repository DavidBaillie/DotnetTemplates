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
    /// Name for the test scheme
    /// </summary>
    public const string AuthenticationScheme = "Test-Authentication";

    /// <summary>
    /// Secret key used to sign the token. This can be changed to anything.
    /// </summary>
    private const string secretKey = "a355e16e-331c-49d4-b24f-bb4e442aad5a";

    /// <summary>
    /// Issuer for the token. If you have custom requirements, change this.
    /// </summary>
    public const string TestIssuer = "someIssuer";

    /// <summary>
    /// Audience for the token. If you have custom requirements, change this.
    /// </summary>
    public const string TestAudience = "someAudience";

    /// <summary>
    /// Generates a signed JWT for use as a Bearer token. 
    /// </summary>
    /// <param name="claims">Provided collection of claims to add to the token</param>
    /// <returns></returns>
    public static string GenerateJwtToken(IDictionary<string, string> claims)
    {
        // Create token
        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: [.. claims.Select(x => new Claim(x.Key, x.Value))],
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), SecurityAlgorithms.HmacSha256));

        // Return the token as a string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Validate the inbound token and generate a <see cref="AuthenticationTicket"/> to represent it.
    /// </summary>
    /// <param name="token">Token to validate into an authentication ticket</param>
    /// <returns></returns>
    public static AuthenticationTicket ValidateIssuedToken(this string token)
    {
        // Don't map inbound claims to the claim names that Microsoft likes to use.
        // This allows all claim keys to remain unchanged
        var tokenHandler = new JwtSecurityTokenHandler() { MapInboundClaims = false };

        // Validate nothing, we blind trust the token
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

        return new AuthenticationTicket(principal, AuthenticationScheme);
    }
}
