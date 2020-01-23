using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.Shares;
using BitContainer.Contracts.V1.Storage;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Controllers.Ui;
using BitContainer.Presentation.Helpers;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels.Enums;
using BitContainer.Presentation.ViewModels.Nodes;
using BitContainer.Shared.Models;
using NUnit.Framework;

namespace BitContainer.UnitTests
{
    [TestFixture]
    public class ArrangementTests
    {
        private ArrangeController _arrangeController;

        [SetUp]
        public void SetUp()
        {
            Guid mockOwnerId = Guid.NewGuid();
            Guid mockParentId = Guid.Empty;

            CStorageEntityContract[] dirs = new CStorageEntityContract[]
            {
                new CStorageEntityContract(EStorageEntityTypeContract.Directory, id:Guid.NewGuid(), parentId:mockParentId, 
                    ownerId:mockOwnerId, name:"FolderA",  created:DateTime.Parse("2019-10-27T21:21:29"), size:0), 
                new CStorageEntityContract(EStorageEntityTypeContract.Directory, id:Guid.NewGuid(), parentId:mockParentId, 
                    ownerId:mockOwnerId, name:"FolderB", created:DateTime.Parse("2019-10-27T21:26:43"), size:0), 
                new CStorageEntityContract(EStorageEntityTypeContract.Directory, id:Guid.NewGuid(), parentId:mockParentId, 
                    ownerId:mockOwnerId, name:"FolderC", created:DateTime.Parse("2019-11-05T12:30:38"), size:0),  
                new CStorageEntityContract(EStorageEntityTypeContract.Directory, id:Guid.NewGuid(), parentId:mockParentId, 
                    ownerId:mockOwnerId, name:"FolderD", created:DateTime.Parse("2019-11-05T12:30:45"), size:0), 
                new CStorageEntityContract(EStorageEntityTypeContract.Directory, id:Guid.NewGuid(), parentId:mockParentId, 
                    ownerId:mockOwnerId, name:"FolderE", created:DateTime.Parse("2019-11-05T12:30:51"), size:0),  
                new CStorageEntityContract(EStorageEntityTypeContract.Directory, id:Guid.NewGuid(), parentId:mockParentId, 
                    ownerId:mockOwnerId, name:"FolderF", created:DateTime.Parse("2019-11-05T12:30:59"), size:0), 
            };

            CStorageEntityContract[] files = new CStorageEntityContract[]
            {
                new CStorageEntityContract(EStorageEntityTypeContract.File, id:Guid.NewGuid(), parentId:mockParentId, ownerId:mockOwnerId, 
                    name:"file6.txt", created:DateTime.Parse("2019-10-28T14:31:49"), size:1000), 
                new CStorageEntityContract(EStorageEntityTypeContract.File, id:Guid.NewGuid(), parentId:mockParentId, ownerId:mockOwnerId, 
                    name:"file5.ico", created:DateTime.Parse("2019-10-28T14:32:46"), size:1000), 
                new CStorageEntityContract(EStorageEntityTypeContract.File, id:Guid.NewGuid(), parentId:mockParentId, ownerId:mockOwnerId, 
                    name:"file4.pdf", created:DateTime.Parse("2019-10-28T14:33:15"), size:1000), 
                new CStorageEntityContract(EStorageEntityTypeContract.File, id:Guid.NewGuid(), parentId:mockParentId, ownerId:mockOwnerId, 
                    name:"file3.jpg", created:DateTime.Parse("2019-11-05T12:37:02"), size:1000), 
                new CStorageEntityContract(EStorageEntityTypeContract.File, id:Guid.NewGuid(), parentId:mockParentId, ownerId:mockOwnerId, 
                    name:"file2.jpg", created:DateTime.Parse("2019-11-05T12:40:43"), size:1000), 
                new CStorageEntityContract(EStorageEntityTypeContract.File, id:Guid.NewGuid(), parentId:mockParentId, ownerId:mockOwnerId, 
                    name:"file1.jpg", created:DateTime.Parse("2019-11-05T12:40:53"), size:1000),
            };

            List<CAccessWrapperContract> oneFileSystemTierEntities = new List<CAccessWrapperContract>();

            foreach (var dir in dirs)
                oneFileSystemTierEntities.Add(new CAccessWrapperContract(dir, EAccessType.Write));

            foreach (var file in files)
                oneFileSystemTierEntities.Add(new CAccessWrapperContract(file, EAccessType.Write));

            List<FileSystemNode> nodes = new List<FileSystemNode>();
            FileSystemController systemController = DependecyController.GetFileSystemController();
            foreach (var accessContract in oneFileSystemTierEntities)
            {
                CSharableEntityUi sharable = new CSharableEntityUi(
                        ContractsConverter.Convert(accessContract.EntityContract), 
                        accessContract.Access, 
                        new List<CShareUi>());

                var node = new FileSystemNode(systemController.Root, sharable);
                nodes.Add(node);
            }

            _arrangeController = new ArrangeController(nodes, GroupType.None, SortType.Name);
        }

        [Test]
        public async Task GroupStorageEntitiesByCreationDate()
        {
            Dictionary<String, ObservableCollection<FileSystemNode>> result =
                await _arrangeController.Arrange(GroupType.Created);

            var groupKey1 = new DateTime(2019, 11, 05).ToLongDateString();
            var groupKey2 = new DateTime(2019, 10, 28).ToLongDateString();
            var groupKey3 = new DateTime(2019, 10, 27).ToLongDateString();

            Assert.That(result.Keys.Count, Is.EqualTo(3));
            Assert.That(result.Keys, Is.EquivalentTo(new String[]
            {
                groupKey1, groupKey2, groupKey3
            }));

            Assert.That(result[groupKey1].Count, Is.EqualTo(7));
            Assert.That(result[groupKey2].Count, Is.EqualTo(3));
            Assert.That(result[groupKey3].Count, Is.EqualTo(2));
        }

        [Test]
        public async Task SortStorageEntitiesByCreationTime()
        {
            Dictionary<String, ObservableCollection<FileSystemNode>> result = await 
                _arrangeController.Arrange(GroupType.None, SortType.Created);

            Assert.That(result[String.Empty].First().CreatedDateTime, Is.EqualTo(
                new DateTime(2019, 11, 05, 12, 40, 53)
            ));
            Assert.That(result[String.Empty].Last().CreatedDateTime, Is.EqualTo(
                new DateTime(2019, 10, 27, 21, 21, 29)
            ));
        }

        [Test]
        public async Task SortStorageEntitiesByName()
        {
            Dictionary<String, ObservableCollection<FileSystemNode>> result = await 
                _arrangeController.Arrange(GroupType.None, SortType.Name);
            Assert.That(result[String.Empty].First().Name, Is.EqualTo("FolderF"));
            Assert.That(result[String.Empty].Last().Name, Is.EqualTo("file1.jpg"));
        }
    }
}
