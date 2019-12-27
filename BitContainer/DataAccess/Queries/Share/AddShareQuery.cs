using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Add
{
    public class AddShareQuery : AbstractWriteQuery
    {
        public Guid PersonApprovedId { get; set; }
        public ERestrictedAccessType RestrictedAccessType { get; set; }
        public Guid DirectoryId { get; set; }
        
        private static readonly String QueryString =
            $"INSERT INTO {DbNames.Shares} " +
            $"({DbNames.Shares.PersonApprovedId}, " +
            $"{DbNames.Shares.AccessTypeId}, " +
            $"{DbNames.Shares.EntityId}) " +
            $"VALUES " +
            $"(@{nameof(PersonApprovedId)}, " +
            $"@{nameof(RestrictedAccessType)}, " +
            $"@{nameof(DirectoryId)})";

        public AddShareQuery(
            Guid personApprovedId, 
            ERestrictedAccessType restrictedAccessType,
            Guid directoryId)
        {
            PersonApprovedId = personApprovedId;
            RestrictedAccessType = restrictedAccessType;
            DirectoryId = directoryId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(PersonApprovedId), PersonApprovedId);
            command.Parameters.AddWithValue(nameof(RestrictedAccessType), RestrictedAccessType.ToInt32());
            command.Parameters.AddWithValue(nameof(DirectoryId), DirectoryId);
            return command;
        }
    }
}
