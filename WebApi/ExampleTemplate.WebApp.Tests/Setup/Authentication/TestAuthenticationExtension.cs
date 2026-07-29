using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleTemplate.WebApp.Tests.Setup.Authentication;

public static class TestAuthenticationExtension
{
    /// <summary>
    /// Configures the Http Client for the Web Application Factory to use a custom JWT as 
    /// a Bearer token during each request. 
    /// </summary>
    /// <param name="client">Client to configure</param>
    /// <returns>Configured client</returns>
    public static HttpClient ConfigureTestAuthentication(this HttpClient client)
    {
        var token = AuthenticationTokenGenerator.GenerateJwtToken(new Dictionary<string, string>()
        {
            { "aud", AuthenticationTokenGenerator.TestAudience },
            { "iss", AuthenticationTokenGenerator.TestIssuer }
        });
        client.DefaultRequestHeaders.Authorization = new($"Bearer", token);

        return client;
    }

    /// <summary>
    /// Configures the test authentication scheme for the runtime to use.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = AuthenticationTokenGenerator.AuthenticationScheme;
            x.DefaultChallengeScheme = AuthenticationTokenGenerator.AuthenticationScheme;
        })
        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(AuthenticationTokenGenerator.AuthenticationScheme, op => { });

        return services;
    }
}