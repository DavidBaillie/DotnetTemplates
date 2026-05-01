using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SukkotStore.WebApp.Database.EntityFramework;
using SukkotStore.WebApp.Database.Mapping;
using SukkotStore.WebApp.Models.Requests;

namespace SukkotStore.WebApp.Endpoints.Users;

public static class CreateUser
{
    public static async Task<IResult> CreateAsync(
        [FromBody] CreateUserRequest request,
        [FromServices] IDbContextFactory<AppDbContext> factory,
        CancellationToken cancellationToken)
    {
        using var context = await factory.CreateDbContextAsync(cancellationToken);
        var entry = await context.Users.AddAsync(request.ToEntity(), cancellationToken);
        await entry.ReloadAsync(cancellationToken);

        return TypedResults.CreatedAtRoute(
                routeName: GetUserById.GET_USER_ROUTE_NAME,
                routeValues: new { id = entry.Entity.Id },
                value: entry.Entity.ToDto());
    }
}
