using CollectorsApp.Models;

namespace CollectorsApp.Services.User
{
    public interface IUserService
    {
        Task<string> RegisterUserAsync(Users user);
    }
}
