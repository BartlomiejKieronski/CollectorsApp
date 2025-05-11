using CollectorsApp.Models;
using ServiceStack;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IPwdReset
    {
        Task PostPasswordReset(PasswordResetModel model);
        Task PutPasswordReset(PasswordResetModel model);
        Task DeletePasswordReset(int id);
        Task<PasswordResetModel> GetPasswordResetModelByEmail(string email);
    }
}
