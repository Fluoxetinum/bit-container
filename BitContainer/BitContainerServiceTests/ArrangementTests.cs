using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BitContainer.Contracts.V1.Shares;
using BitContainer.Contracts.V1.Storage;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Controllers.Ui;
using BitContainer.Presentation.Helpers;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels.Nodes;
using NUnit.Framework;

namespace BitContainer.UnitTests
{
    [TestFixture]
    public class ArrangementTests
    {
        private Dictionary<String, ObservableCollection<IAccessWrapperUiModel>> _oneTierEntities;
        private Dictionary<String, ObservableCollection<CFileSystemNode>> _oneTierFileNodes;

        [SetUp]
        public void SetUp()
        {
            Guid mockOwnerId = Guid.NewGuid();
            Guid mockParentId = Guid.Empty;

            CDirectoryContract[] dirs = new CDirectoryContract[]
            {
                new CDirectoryContract(id:Guid.NewGuid(), parentId:mockParentId, 
                    ownerId:mockOwnerId, name:"FolderA",  created:DateTime.Parse("2019-10-27T21:21:29")), 
                new CDirectoryContract(id:Guid.NewGuid(), parentId:mockParentId, 
                    ownerId:mockOwnerId, name:"FolderB", created:DateTime.Parse("2019-10-27T21:26:43")), 
                new CDirectoryContract(id:Guid.NewGuid(), parentId:mockParentId, 
                    ownerId:mockOwnerId, name:"FolderC", created:DateTime.Parse("2019-11-05T12:30:38")), 
                new CDirectoryContract(id:Guid.NewGuid(), parentId:mockParentId, 
                    ownerId:mockOwnerId, name:"FolderD", created:DateTime.Parse("2019-11-05T12:30:45")), 
                new CDirectoryContract(id:Guid.NewGuid(), parentId:mockParentId, 
                    ownerId:mockOwnerId, name:"FolderE", created:DateTime.Parse("2019-11-05T12:30:51")), 
                new CDirectoryContract(id:Guid.NewGuid(), parentId:mockParentId, 
                    ownerId:mockOwnerId, name:"FolderF", created:DateTime.Parse("2019-11-05T12:30:59")),
            };

            CFileContract[] files = new CFileContract[]
            {
                new CFileContract(id:Guid.NewGuid(), parentId:mockParentId, ownerId:mockOwnerId, 
                    name:"file6.txt", created:DateTime.Parse("2019-10-28T14:31:49"), size:1000), 
                new CFileContract(id:Guid.NewGuid(), parentId:mockParentId, ownerId:mockOwnerId, 
                    name:"file5.ico", created:DateTime.Parse("2019-10-28T14:32:46"), size:1000), 
                new CFileContract(id:Guid.NewGuid(), parentId:mockParentId, ownerId:mockOwnerId, 
                    name:"file4.pdf", created:DateTime.Parse("2019-10-28T14:33:15"), size:1000), 
                new CFileContract(id:Guid.NewGuid(), parentId:mockParentId, ownerId:mockOwnerId, 
                    name:"file3.jpg", created:DateTime.Parse("2019-11-05T12:37:02"), size:1000), 
                new CFileContract(id:Guid.NewGuid(), parentId:mockParentId, ownerId:mockOwnerId, 
                    name:"file2.jpg", created:DateTime.Parse("2019-11-05T12:40:43"), size:1000), 
                new CFileContract(id:Guid.NewGuid(), parentId:mockParentId, ownerId:mockOwnerId, 
                    name:"file1.jpg", created:DateTime.Parse("2019-11-05T12:40:53"), size:1000),
            };

            List<COwnStorageEntityContract> oneFileSystemTierEntities = new List<COwnStorageEntityContract>();

            foreach (var dir in dirs)
                oneFileSystemTierEntities.Add(new COwnStorageEntityContract(dir, isShared:false));

            foreach (var file in files)
                oneFileSystemTierEntities.Add(new COwnStorageEntityContract(file, isShared:false));

            _oneTierEntities = new Dictionary<String, ObservableCollection<IAccessWrapperUiModel>>
            {
                [String.Empty] = new ObservableCollection<IAccessWrapperUiModel>()
            };

            _oneTierFileNodes = new Dictionary<string, ObservableCollection<CFileSystemNode>>()
            {
                [String.Empty] = new ObservableCollection<CFileSystemNode>()
            };

            FileSystemController systemController = new FileSystemController();

            foreach (var accessContract in oneFileSystemTierEntities)
            {
                var accessUiModel = ContractsConverter.GetAccessWrapperUiModel(accessContract);
                var node = new CFileSystemNode(systemController.Root, accessUiModel);
                _oneTierEntities[String.Empty].Add(accessUiModel);
                _oneTierFileNodes[String.Empty].Add(node);
            }
        }

        [Test]
        public async Task GroupStorageEntitiesByCreationDate()
        {
            Dictionary<String, ObservableCollection<CFileSystemNode>> result = await 
                ArrangeController.GroupByCreationDate(_oneTierFileNodes);

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
            var input = new Dictionary<String, ObservableCollection<CFileSystemNode>>(_oneTierFileNodes);

            Dictionary<String, ObservableCollection<CFileSystemNode>> result = await 
                ArrangeController.SortByCreationTime(input);

            Assert.That(result[String.Empty].First().CreatedTime, Is.EqualTo(
                new DateTime(2019, 11, 05, 12, 40, 53)
            ));
            Assert.That(result[String.Empty].Last().CreatedTime, Is.EqualTo(
                new DateTime(2019, 10, 27, 21, 21, 29)
            ));
        }

        [Test]
        public async Task SortStorageEntitiesByName()
        {
            var input = new Dictionary<String, ObservableCollection<CFileSystemNode>>(_oneTierFileNodes);

            Dictionary<String, ObservableCollection<CFileSystemNode>> result = await 
                ArrangeController.SortByName(input);
            Assert.That(result[String.Empty].First().Entity.Name, Is.EqualTo("file1.jpg"));
            Assert.That(result[String.Empty].Last().Entity.Name, Is.EqualTo("FolderF"));
        }


        [Test]
        public void FilterStorageEntitiesByName()
        {
            Dictionary<String, ObservableCollection<IAccessWrapperUiModel>> result =
                ArrangeController.FilterByName(_oneTierEntities, "*.txt", "*.jpg");
            
            List<String> names = new List<string>();
            foreach (var accessUiModel in result[String.Empty])
            {
                names.Add(accessUiModel.Entity.Name);
            }

            Assert.That(names.Count, Is.EqualTo(4));
            Assert.That(names, Has.All.EndsWith(".txt").Or.EndsWith(".jpg"));
        }
    }
}
