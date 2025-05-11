using CollectorsApp.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CollectorsApp.Policies
{
    public class EntityOwnerHandler : AuthorizationHandler<EntityOwnerRequirement, IOwner>
    {
        protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        EntityOwnerRequirement requirement,
        IOwner owner)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null && owner.OwnerId == Convert.ToInt32(userId))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
