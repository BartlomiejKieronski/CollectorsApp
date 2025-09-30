using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CollectorsApp.Policies
{
    /// <summary>
    /// Enforces that the user is either the owner of the resource being accessed or has an Admin role.
    /// </summary>
    public class ResourceOwnerHandler : AuthorizationHandler<ResourceOwnerRequirement>
    {
        /// <summary>
        /// Checks if the current user is the owner of the resource being accessed or has an Admin role.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOwnerRequirement requirement)
        {
            
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (userId == null) {
                context.Fail();
                return Task.CompletedTask;
            }
            if (context.Resource is HttpContext httpContext)
            {
                string routeId = httpContext.GetRouteValue("userId")?.ToString();
                if (routeId == null)
                {
                    routeId = httpContext.Request.Query["userId"].FirstOrDefault();
                }
                if (routeId == null)
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
                if (routeId == userId) { 
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            context.Fail();
            return Task.CompletedTask;
        }
    }
}
