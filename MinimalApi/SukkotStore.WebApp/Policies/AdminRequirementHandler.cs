using Microsoft.AspNetCore.Authorization;
using SukkotStore.WebApp.Constants;
using SukkotStore.WebApp.Interfaces;

namespace SukkotStore.WebApp.Policies;

public sealed class AdminRequirementHandler(
    IUserService userService)
    : AuthorizationHandler<AdminRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
            return;

        // Check for a UserId claim
        var subjectClaim = httpContext.User.Claims
            .FirstOrDefault(x => x.Type.Equals("sub", StringComparison.InvariantCultureIgnoreCase))?.Value;
        if (!Guid.TryParse(subjectClaim, out Guid userId))
        {
            context.Fail();
            return;
        }

        // Check the UserId is known in the database
        var existingUser = await userService.GetUserAsync(userId, CancellationToken.None);
        if (existingUser is null)
        {
            context.Fail();
            return;
        }

        // User must be an admin
        if (!existingUser.Roles.Any(x => x == UserRoleConstants.ADMIN_ROLE))
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}
