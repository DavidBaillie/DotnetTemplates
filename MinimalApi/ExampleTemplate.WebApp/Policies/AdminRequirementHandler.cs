using Microsoft.AspNetCore.Authorization;
using ExampleTemplate.WebApp.Constants;

namespace ExampleTemplate.WebApp.Policies;

/// <summary>
/// Authorization handler that checks if the user has the admin role.
/// </summary>
public sealed class AdminRequirementHandler : AuthorizationHandler<AdminRequirement>
{
    /// <inheritdoc />
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminRequirement requirement)
    {
        // Check if the user has the admin role claim
        if (context.User.IsInRole(UserRoleConstants.ADMIN_ROLE))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
