using CollectorsApp.Services.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CollectorsApp.Policies
{
    public class ResourceOwnerHandler : AuthorizationHandler<ResourceOwnerRequirement>
    {
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
