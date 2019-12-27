using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BitContainer.Presentation.Models
{
    public interface IStorageEntityUiModel
    {
        Guid Id { get; set; }
        Guid ParentId { get; set; }
        Guid OwnerId { get; set; }
        String Name {get; set; }
        DateTime Created { get; set; }

        Task Delete();
        Task Rename(String name);
        Task Copy(Guid newParentId);
    }
}
