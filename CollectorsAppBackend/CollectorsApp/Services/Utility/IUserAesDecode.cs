using CollectorsApp.Models;

namespace CollectorsApp.Services.Utility
{
    public interface IUserAesDecode
    {
        Task<Users> GetUserDataFromEncryption(Users hasheduser);
        Task<LoggedUserInfo> LoggedUserDataDecrypt(Users hasheduser);
    }
}
