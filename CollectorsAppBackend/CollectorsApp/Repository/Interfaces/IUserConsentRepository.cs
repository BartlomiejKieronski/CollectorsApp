using CollectorsApp.Filters;
using CollectorsApp.Models;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IUserConsentRepository : IQueryRepository<UserConsent, UserConsentFilter>, IGenericRepository<UserConsent>
    {
        Task<IEnumerable<UserConsent>> GetByUserIdAsync(int userId);
    }
}
