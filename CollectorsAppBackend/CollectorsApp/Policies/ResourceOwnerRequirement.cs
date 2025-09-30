using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace CollectorsApp.Policies
{
    /// <summary>
    /// Represents an authorization requirement that ensures the current user is the owner of a specific resource.
    /// </summary>
    /// <remarks>This requirement is typically used in conjunction with a custom authorization handler to
    /// enforce resource ownership rules. The handler should verify that the user has the necessary ownership rights for
    /// the resource being accessed.</remarks>
    public class ResourceOwnerRequirement : IAuthorizationRequirement
    {

    }
    /// <summary>
    /// Represents a requirement used to verify that the current user is the owner of an entity.
    /// </summary>
    /// <remarks>This requirement is typically used in authorization policies to enforce that a user has
    /// ownership over a specific resource. The requirement is identified by the name "OwnerId".</remarks>
    public class EntityOwnerRequirement : OperationAuthorizationRequirement
    {
        public EntityOwnerRequirement() { Name = "OwnerId"; }
    }
}
