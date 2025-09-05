using CollectorsApp.Models;
using ServiceStack;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IPwdResetRepository : ICRUD<PasswordReset>
    {
        Task<PasswordReset> GetPasswordResetModelByEmail(string email);
    }
}
