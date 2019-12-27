using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.DataAccess.Models;

namespace BitContainer.DataAccess.DataProviders.Interfaces
{
    public interface IStorageEntitiesProvider
    {
        List<COwnStorageEntity> GetOwnerChildren(Guid parentDirectoryId, Guid ownerId);
        List<CRestrictedStorageEntity> GetSharedChildren(Guid parentDirectoryId, Guid userId);
        Dictionary<Int32, List<IStorageEntity>> GetAllChildren(Guid dirId);

        Int32 RenameEntity(Guid id, String name);

        CDirectory GetDir(Guid parentId, Guid userId, String name);
        CDirectory GetDir(Guid id);

        CDirectory AddDir(Guid parentId, Guid userId, String name);
        Int32 DeleteDir(Guid id);
        Int32 CopyDir(Guid id, Guid copierId, Guid newParentId);

        CFile GetFile(Guid parentId, Guid getterId, String name);
        CFile GetFile(Guid id);

        Byte[] GetAllFileData(Guid fileId);

        CFile AddFile(Guid parentId, Guid adderId, String name, Byte[] data);
        Int32 DeleteFile(Guid id);
        Int32 CopyFile(Guid id, Guid copierId, Guid newParentId);
    }
}
