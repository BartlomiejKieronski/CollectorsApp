using CollectorsApp.Data;
using CollectorsApp.Filters;
using CollectorsApp.Models.APILogs;
using CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces;

namespace CollectorsApp.Repository.AnalyticsRepositories
{
    public class APILogRepository : QueryRepository<APILog, APILogFilter>, IAPILogRepository
    {
        public APILogRepository(appDatabaseContext context) : base(context)
        {

        }

        
    }
}
