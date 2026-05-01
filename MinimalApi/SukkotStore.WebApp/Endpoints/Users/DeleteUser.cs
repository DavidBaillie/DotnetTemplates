using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SukkotStore.WebApp.Database.EntityFramework;

namespace SukkotStore.WebApp.Endpoints.Users;

public static class DeleteUser
{
    public static async Task<IResult> DeleteAsync(
        [FromRoute] Guid id,
        [FromServices] IDbContextFactory<AppDbContext> factory,
        CancellationToken cancellationToken)
    {
        using var context = await factory.CreateDbContextAsync(cancellationToken);
        await context.Users
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
