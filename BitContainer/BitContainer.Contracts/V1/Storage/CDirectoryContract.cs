using System;

namespace BitContainer.Contracts.V1.Storage
{
    public class CDirectoryContract : IStorageEntityContract
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set;}
        public Guid OwnerId { get; set;}
        public String Name { get; set;}
        public DateTime Created { get; set;}

        private CDirectoryContract () {}

        public CDirectoryContract (Guid id, Guid parentId, Guid ownerId, String name, DateTime created)
        {
            Id = id;
            ParentId = parentId;
            OwnerId = ownerId;
            Name = name;
            Created = created;
        }

        public static CDirectoryContract CreateBlank(Guid parentId, String name)
        {
            return new CDirectoryContract(
                id:Guid.Empty,
                ownerId:Guid.Empty, 
                created:DateTime.MaxValue,
                parentId:parentId,
                name:name);
        }
    }
}
