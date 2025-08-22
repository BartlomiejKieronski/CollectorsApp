using CollectorsApp.Models.Analytics;
using CollectorsApp.Models.Filters;
using CollectorsApp.Repository.Interfaces;

namespace CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces
{
    public interface IAdminCommentsRepository : IQueryInterface<AdminComment, AdminCommentFilter>, ICRUD<AdminComment>
    {
        
    }
}
