﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.Stats
{
    public class AddNewStatsQuery : AbstractWriteQuery
    {
        public CUserId UserId { get; set; }

        private static readonly String Query = 
            $"INSERT INTO {DbNames.Stats} " +
            $"({DbNames.Stats.PxUserId}, {DbNames.Stats.PxFilesCount}, " +
            $"{DbNames.Stats.PxDirsCount}, {DbNames.Stats.PxStorageSize}) " +
            $"VALUES (@{nameof(UserId)}, 0, 0, 0)";

        public AddNewStatsQuery(CUserId userId)
        {
            UserId = userId;
        }
        
        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = Query;
            command.Parameters.AddWithValue(nameof(UserId), UserId.ToGuid());
            return command;
        }
    }
}
