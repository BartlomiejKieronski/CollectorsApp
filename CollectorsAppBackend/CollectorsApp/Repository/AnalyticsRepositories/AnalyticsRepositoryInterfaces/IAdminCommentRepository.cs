using CollectorsApp.Filters;
using CollectorsApp.Models.Analytics;
using CollectorsApp.Repository.Interfaces;

namespace CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces
{
    public interface IAdminCommentsRepository : IQueryInterface<AdminComment, AdminCommentFilter>, ICRUD<AdminComment>
    {
        
    }
}
