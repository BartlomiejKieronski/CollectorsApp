using CollectorsApp.Filters;
using CollectorsApp.Models.Analytics;
using CollectorsApp.Repository.Interfaces;

namespace CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces
{
    public interface IAdminCommentRepository : IQueryRepository<AdminComment, AdminCommentFilter>, IGenericRepository<AdminComment>
    {
        
    }
}
