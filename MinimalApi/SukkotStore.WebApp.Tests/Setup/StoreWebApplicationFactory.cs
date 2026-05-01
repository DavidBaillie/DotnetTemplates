using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace SukkotStore.WebApp.Tests.Setup;

public sealed class StoreWebApplicationFactory(TestUserContext? context)
    : WebApplicationFactory<SukkotStore.WebApp.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        // Modify the DI container here
        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = "test";
                x.DefaultChallengeScheme = "test";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("test", op => { });
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);

        if (context is null)
            return;

        var token = AuthenticationTokenGenerator.GenerateJwtToken(new Dictionary<string, string>()
        {
            { "email", context.Email },
            { "sub",  context.UserId.ToString() },
            { "aud", AuthenticationTokenGenerator.audience },
            { "iss", AuthenticationTokenGenerator.issuer }
        });
        client.DefaultRequestHeaders.Authorization = new($"Bearer", token);
    }
}
