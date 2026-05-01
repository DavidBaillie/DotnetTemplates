using SukkotStore.WebApp.Constants;
using SukkotStore.WebApp.Models.Options;

namespace SukkotStore.WebApp.Extensions;

public static class RegisterHttpClientExtensions
{
    public static IServiceCollection RegisterHttpClients(this IServiceCollection services, AuthenticationSettings settings)
    {
        services.AddHttpClient(HttpClientNames.AuthenticationClient, client =>
        {
            client.BaseAddress = new Uri(settings.AuthenticationServerEndpoint);
        });

        return services;
    }
}
