using CollectorsApp.Filters;
using CollectorsApp.Models.APILogs;
using CollectorsApp.Repository.Interfaces;

namespace CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces
{
    public interface IAPILogRepository : IQueryRepository<APILog, APILogFilter>
    {
    }
}
