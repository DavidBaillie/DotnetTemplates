using Microsoft.AspNetCore.Authorization;

namespace ExampleTemplate.WebApp.Policies;

/// <summary>
/// Authorization requirement that validates a user has administrative privileges.
/// </summary>
public sealed class AdminRequirement : IAuthorizationRequirement;
