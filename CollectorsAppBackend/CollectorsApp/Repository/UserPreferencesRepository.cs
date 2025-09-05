using CollectorsApp.Data;
using CollectorsApp.Filters;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;

namespace CollectorsApp.Repository
{
    public class UserPreferencesRepository : QueryRepository<UserPreferences, UserPreferencesFilter>, IUserPreferencesRepository
    {
        public UserPreferencesRepository(appDatabaseContext context) : base(context)
        {

        }
    }
}
