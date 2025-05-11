using CollectorsApp.Models;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<Users>> GetUsers();
        Task<Users> GetUser(int id);
        Task<string> PostUser(Users user);
        Task UpdateUser(Users user, int id);
        Task DeleteUser(int id);
        Task<Users> GetUserByNameOrEmailAsync(LoginInfo user);
    }
}
