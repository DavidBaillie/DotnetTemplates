using Microsoft.Extensions.DependencyInjection;

namespace ExampleTemplate.Application.Extensions;

public static class RegisterApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services;
    }
}