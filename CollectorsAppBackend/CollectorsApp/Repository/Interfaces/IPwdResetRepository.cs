using CollectorsApp.Models;
using ServiceStack;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IPwdResetRepository : ICRUD<PasswordResetModel>
    {
        Task<PasswordResetModel> GetPasswordResetModelByEmail(string email);
    }
}
