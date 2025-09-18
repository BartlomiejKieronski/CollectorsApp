using CollectorsApp.Models;
using CollectorsApp.Models.DTO.Auth;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IUserRepository : IGenericRepository<Users>
    {
        Task<string> PostUser(Users user);
        Task<Users> GetUserByNameOrEmailAsync(LoginRequest user);
    }
}
