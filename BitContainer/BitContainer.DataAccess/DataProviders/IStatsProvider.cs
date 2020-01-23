using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.DataProviders.Interfaces
{
    public interface IStatsProvider
    {
        void AddNewStats(SqlCommand command, CUserId userId);
        void RemoveStats(SqlCommand command, CUserId userId);
        void UpdateStats(SqlCommand command, CStats stats);
        CStats GetStats(SqlCommand command, CUserId userId);
         
        CStats GetStats(CUserId userId);
    }
}
