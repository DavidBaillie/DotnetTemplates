using SukkotStore.WebApp.Constants;
using SukkotStore.WebApp.Endpoints.Authentication;
using SukkotStore.WebApp.Endpoints.Users;

namespace SukkotStore.WebApp.Extensions;

public static class MapEndpointsExtensions
{
    public static IEndpointRouteBuilder RegisterEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // AUTH
        endpoints.MapPost("api/auth/login", Login.LoginAsync)
            .AllowAnonymous();
        endpoints.MapPost("api/auth/refresh", Refresh.RefreshAsync)
            .AllowAnonymous();

        // USER CRUD
        endpoints.MapGet("api/users/{id:guid}", GetUserById.GetAsync)
            .WithName(GetUserById.GET_USER_ROUTE_NAME)
            .AllowAnonymous();
        endpoints.MapPost("api/users", CreateUser.CreateAsync)
            .RequireAuthorization(PolicyNameConstants.ADMIN_POLICY);
        endpoints.MapPut("api/users/{id:guid}", UpdateUser.UpdateAsync)
            .RequireAuthorization(PolicyNameConstants.ADMIN_POLICY);
        endpoints.MapDelete("api/users/{id:guid}", DeleteUser.DeleteAsync)
            .RequireAuthorization(PolicyNameConstants.ADMIN_POLICY);

        return endpoints;
    }
}
