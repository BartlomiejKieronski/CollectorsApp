using CollectorsApp.Data;
using CollectorsApp.Filters;
using CollectorsApp.Models.Analytics;
using CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CollectorsApp.Repository.AnalyticsRepositories
{
    public class AdminCommentsRepository : QuerryRepository<AdminComment, AdminCommentFilter>, IAdminCommentsRepository
    {
        public AdminCommentsRepository(appDatabaseContext context) : base(context) 
        {
        }

        public override async Task<IEnumerable<AdminComment>> QueryEntity(AdminCommentFilter entity)
        {
            if (entity == null)
                return null;

            IQueryable<AdminComment> query = _dbSet.AsQueryable();
            
            if(entity.Id.HasValue) 
                query = query.Where(x=> x.Id == entity.Id);
            
            if(entity.AdminId.HasValue) 
                query = query.Where(x=>x.AdminId == entity.AdminId);
            
            if(!entity.TargetType.IsNullOrEmpty()) 
                query = query.Where(x=>x.TargetType == entity.TargetType);
            
            if(!entity.CommentText.IsNullOrEmpty())
                query = query.Where(x=>x.CommentText == entity.CommentText);
            
            if(entity.TimeStamp.HasValue)
                query = query.Where(x=>x.TimeStamp == entity.TimeStamp);
            
            if(entity.EventLogId.HasValue)
                query=query.Where(x=>x.EventLogId == entity.EventLogId);

            if(entity.Before.HasValue)
                query=query.Where(x=>x.TimeStamp <= entity.Before);

            if(entity.After.HasValue)
                query=query.Where(x=>x.TimeStamp >= entity.After);

            if (!entity.OrderBy.IsNullOrEmpty())
                query = query.OrderByDescending(x=>entity.OrderBy);

            return await query.ToListAsync();
        }
    }
}