//#if(includeAuth)
using ExampleTemplate.WebApp.Tests.Setup.Authentication;
//#endif
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ExampleTemplate.WebApp.Tests.Setup;

/// <summary>
/// Web Application factory used to spin up instances of the WebApp.
/// </summary>
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
            services.ConfigureAuthentication();
            //#endif
        });
    }

    /// <inheritdoc />
    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);

        //#if(includeAuth)
        client.ConfigureTestAuthentication();
        //#endif
    }
}
