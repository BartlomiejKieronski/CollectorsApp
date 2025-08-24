using CollectorsApp.Data;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;

namespace CollectorsApp.Repository
{
    public class UserConsentRepository : CRUDImplementation<UserConsent>, IUserConsentRepository
    {
        public UserConsentRepository(appDatabaseContext context) : base(context)
        {

        }
    }
}
