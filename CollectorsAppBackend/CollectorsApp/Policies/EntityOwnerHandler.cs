using CollectorsApp.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CollectorsApp.Policies
{
    /// <summary>
    /// Enforces that the current user is the owner of the entity or has an Admin role.
    /// </summary>
    public class EntityOwnerHandler : AuthorizationHandler<EntityOwnerRequirement, IOwner>
    {
        /// <summary>
        /// Checks if the current user is the owner of the entity or has an Admin role.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <param name="owner"></param>
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
