using CollectorsApp.Data;
using CollectorsApp.Filters;
using CollectorsApp.Models.APILogs;
using CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces;

namespace CollectorsApp.Repository.AnalyticsRepositories
{
    /// <summary>
    /// Concrete repository for managing <see cref="APILog"/> entities.
    /// </summary>
    public class APILogRepository : QueryRepository<APILog, APILogFilter>, IAPILogRepository
    {
        public APILogRepository(appDatabaseContext context) : base(context)
        {

        }

        
    }
}
