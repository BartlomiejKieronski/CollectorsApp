using CollectorsApp.Data;
using CollectorsApp.Models.APILogs;
using CollectorsApp.Models.Filters;
using CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces;

namespace CollectorsApp.Repository.AnalyticsRepositories
{
    public class APILogRepository : QuerryRepository<APILog, APILogFilter>, IAPILogRepository
    {
        public APILogRepository(appDatabaseContext context) : base(context)
        {

        }

        public override async Task<IEnumerable<APILog>> QueryEntity(APILogFilter entity)
        {
            throw new NotImplementedException();
        }
    }
}
