using CollectorsApp.Models;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IUserConsentRepository : IGenericRepository<UserConsent>
    {
        Task<IEnumerable<UserConsent>> GetByUserIdAsync(int userId);
    }
}
