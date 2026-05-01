using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SukkotStore.WebApp.Database.EntityFramework;
using SukkotStore.WebApp.Database.Mapping;

namespace SukkotStore.WebApp.Endpoints.Users;

public static class GetUserById
{
    public const string GET_USER_ROUTE_NAME = "GetUserById";

    public static async Task<IResult> GetAsync(
        [FromRoute] Guid id,
        [FromServices] IDbContextFactory<AppDbContext> factory,
        CancellationToken cancellationToken)
    {
        using var context = await factory.CreateDbContextAsync(cancellationToken);
        var userEntity = await context.Users
            .FindAsync([id], cancellationToken);

        if (userEntity is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(userEntity.ToDto());
    }
}
