using CollectorsApp.Models;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IUserRepository : ICRUD<Users>
    {
        Task<string> PostUser(Users user);
        Task<Users> GetUserByNameOrEmailAsync(LoginInfo user);
    }
}
