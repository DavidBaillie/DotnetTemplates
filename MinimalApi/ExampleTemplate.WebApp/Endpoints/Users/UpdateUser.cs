using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SukkotStore.WebApp.Database.EntityFramework;
using SukkotStore.WebApp.Database.Mapping;
using SukkotStore.WebApp.Models.Requests;

namespace SukkotStore.WebApp.Endpoints.Users;

public static class UpdateUser
{
    public static async Task<IResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateUserRequest request,
        [FromServices] IDbContextFactory<AppDbContext> factory,
        CancellationToken cancellationToken)
    {
        using var context = await factory.CreateDbContextAsync(cancellationToken);
        var userEntity = await context.Users
            .Include(x => x.Roles)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (userEntity is null)
            return TypedResults.NotFound();

        userEntity.DisplayName = request.Name;
        userEntity.Roles = request.Roles;

        await context.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok(userEntity.ToDto());
    }
}
