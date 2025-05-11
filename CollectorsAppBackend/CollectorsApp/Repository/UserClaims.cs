using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using System.Security.Claims;

namespace CollectorsApp.Repository
{
    public class UserClaims : IUserClaims
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserClaims(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<bool> IsRequestValidForUser(int id)
        {
            var claims = _httpContextAccessor.HttpContext?.User;
            if (claims == null)
            {
                return Task.FromResult(false);
            }
            var claimsIdentity = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (claimsIdentity == null) {
                return Task.FromResult(false);
            }
            if(Convert.ToInt16(claimsIdentity) == id)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        public async Task<LoggedUserInfo> HttpContextData() 
        {
            var claims = _httpContextAccessor.HttpContext?.User;
            if (claims == null)
            {
                return null;
            }
            LoggedUserInfo loggedUserInfo = new LoggedUserInfo();
            loggedUserInfo.Id = Convert.ToInt32(claims.FindFirstValue(ClaimTypes.NameIdentifier));
            return loggedUserInfo;
        }
    }
}
