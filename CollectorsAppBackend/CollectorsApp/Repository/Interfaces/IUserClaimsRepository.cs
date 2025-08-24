using CollectorsApp.Models;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IUserClaimsRepository
    {
        Task<bool> IsRequestValidForUser(int id);
        Task<LoggedUserInfo> HttpContextData();
    }
}
