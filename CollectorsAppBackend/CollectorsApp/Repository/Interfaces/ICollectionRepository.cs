﻿using CollectorsApp.Models;

namespace CollectorsApp.Repository.Interfaces
{
    public interface ICollectionRepository : ICRUD<Collections>
    {
        Task<IEnumerable<Collections>> GetCollectionsByUserId(int userId);
        Task<IEnumerable<Collections>> GetCollectionsByUserId(int userId, string name);
        Task<bool> DetermineChildComponent(int id);
        Task<bool> IsCollectionNameForUserUnique(int userId, string name);
    }
}
