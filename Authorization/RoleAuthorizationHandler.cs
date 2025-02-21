namespace SafeVault.Authorization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
    {
        if (!context.User.HasClaim(c => c.Type == ClaimTypes.Role))
            return Task.CompletedTask;

        var roles = context.User.FindAll(c => c.Type == ClaimTypes.Role)
                            .Select(c => c.Value);

        if (roles.Any(r => r == requirement.Role))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

public class RoleRequirement : IAuthorizationRequirement
{
    public string Role { get; }
    public RoleRequirement(string role)
    {
        Role = role;
    }
}
