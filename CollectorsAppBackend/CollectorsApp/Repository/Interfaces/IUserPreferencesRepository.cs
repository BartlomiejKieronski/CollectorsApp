using CollectorsApp.Filters;
using CollectorsApp.Models;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IUserPreferencesRepository : IQueryRepository<UserPreferences, UserPreferencesFilter>, IGenericRepository<UserPreferences>
    {

    }
}
