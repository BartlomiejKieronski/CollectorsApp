using CollectorsApp.Data;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    public class UserConsentRepository : CRUDImplementation<UserConsent>, IUserConsentRepository
    {
        public UserConsentRepository(appDatabaseContext context) : base(context)
        {
            

        }
        public async Task<IEnumerable<UserConsent>> GetByUserIdAsync(int userId)
        {
            return await _context.UserConsents.Where(x => x.OwnerId == userId).ToListAsync();
        }


    }
}
