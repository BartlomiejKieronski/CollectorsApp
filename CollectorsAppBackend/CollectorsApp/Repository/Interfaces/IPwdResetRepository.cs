using CollectorsApp.Models;
using ServiceStack;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IPwdResetRepository : IGenericRepository<PasswordReset>
    {
        Task<PasswordReset> GetPasswordResetModelByEmail(string email);
    }
}
