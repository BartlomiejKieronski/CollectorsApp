using CollectorsApp.Models;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IUserClaims
    {
        Task<bool> IsRequestValidForUser(int id);
        Task<LoggedUserInfo> HttpContextData();
    }
}
