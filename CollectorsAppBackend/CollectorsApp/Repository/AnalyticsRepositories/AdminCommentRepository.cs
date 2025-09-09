﻿using CollectorsApp.Data;
using CollectorsApp.Filters;
using CollectorsApp.Models.Analytics;
using CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CollectorsApp.Repository.AnalyticsRepositories
{
    public class AdminCommentRepository : QueryRepository<AdminComment, AdminCommentFilter>, IAdminCommentRepository
    {
        public AdminCommentRepository(appDatabaseContext context) : base(context) 
        {

        }

        
    }
}