using ExampleTemplate.WebApp.Tests.Setup.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleTemplate.WebApp.Tests.Setup;

public sealed class CustomWebApplicationFactory
    : WebApplicationFactory<ExampleTemplate.WebApp.Program>
{
    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        // Modify the DI container here
        builder.ConfigureServices(services =>
        {
            //#if(includeAuth)
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = "test";
                x.DefaultChallengeScheme = "test";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("test", op => { });
            //#endif
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);


    }
}
