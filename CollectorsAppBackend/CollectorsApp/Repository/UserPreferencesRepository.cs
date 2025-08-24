using CollectorsApp.Data;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;

namespace CollectorsApp.Repository
{
    public class UserPreferencesRepository : CRUDImplementation<UserPreferences>, IUserPreferencesRepository
    {
        public UserPreferencesRepository(appDatabaseContext context) : base(context)
        {

        }
    }
}
