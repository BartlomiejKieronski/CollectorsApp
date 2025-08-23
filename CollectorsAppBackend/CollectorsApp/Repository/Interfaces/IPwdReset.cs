using CollectorsApp.Models;
using ServiceStack;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IPwdReset : ICRUD<PasswordResetModel>
    {
        Task<PasswordResetModel> GetPasswordResetModelByEmail(string email);
    }
}
