using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace CollectorsApp.Policies
{
    public class ResourceOwnerRequirement : IAuthorizationRequirement
    {

    }
    public class EntityOwnerRequirement : OperationAuthorizationRequirement
    {
        public EntityOwnerRequirement() { Name = "OwnerId"; }
    }
}
