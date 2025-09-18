using CollectorsApp.Models;
using CollectorsApp.Models.DTO.Auth;

namespace CollectorsApp.Services.Utility
{
    public interface IUserAesDecode
    {
        Task<Users> GetUserDataFromEncryption(Users hasheduser);
        Task<LoggedUserInfo> LoggedUserDataDecrypt(Users hasheduser);
    }
}
