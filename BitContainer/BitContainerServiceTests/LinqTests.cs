using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.Storage;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Helpers;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace BitContainer.Tests
{
    [TestFixture]
    public class LinqTests
    {
        private readonly String _allTiersPath = TestContext.CurrentContext.TestDirectory
                                                + @"\..\..\..\JsonServiceResponses\AllTiers.json";
        private readonly String _oneTierPath = TestContext.CurrentContext.TestDirectory
                                                + @"\..\..\..\JsonServiceResponses\OneTier.json";

        private List<IAccessWrapperUiModel> allTiersEntities;
        private Dictionary<String, ObservableCollection<IAccessWrapperUiModel>> oneTierEntities;
        private Dictionary<String, ObservableCollection<CFileSystemNode>> oneTierFileNodes;

        [SetUp]
        public void SetUp()
        {
            JsonConvert.DefaultSettings = () =>
            {
                JsonSerializerSettings settings = 
                    new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All};
                return settings;
            };

            using (StreamReader text = new StreamReader(File.OpenRead(_allTiersPath)))
            {
                String json = text.ReadToEnd();
                COwnStorageEntitiesListContract contract =
                     JsonConvert.DeserializeObject<COwnStorageEntitiesListContract>(json);

                allTiersEntities = new List<IAccessWrapperUiModel>();

                foreach (var accessContract in contract.Entities)
                {
                    allTiersEntities.Add(ContractsConverter.GetAccessWrapperUiModel(accessContract));
                }
            }

            using (StreamReader text = new StreamReader(File.OpenRead(_oneTierPath)))
            {
                String json = text.ReadToEnd();
                COwnStorageEntitiesListContract contract
                    = JsonConvert.DeserializeObject<COwnStorageEntitiesListContract>(json);

                oneTierEntities = new Dictionary<String, ObservableCollection<IAccessWrapperUiModel>>
                {
                    [String.Empty] = new ObservableCollection<IAccessWrapperUiModel>()
                };

                oneTierFileNodes = new Dictionary<string, ObservableCollection<CFileSystemNode>>()
                {
                    [String.Empty] = new ObservableCollection<CFileSystemNode>()
                };

                FileSystemController systemController = new FileSystemController();

                foreach (var accessContract in contract.Entities)
                {
                    var accessUiModel = ContractsConverter.GetAccessWrapperUiModel(accessContract);
                    var node = new CFileSystemNode(systemController.Root, accessUiModel);
                    oneTierEntities[String.Empty].Add(accessUiModel);
                    oneTierFileNodes[String.Empty].Add(node);
                }
            }
        }

        [Test]
        public async Task GroupStorageEntitiesByCreationDate()
        {
            Dictionary<String, ObservableCollection<CFileSystemNode>> result = await 
                ArrangeController.GroupByCreationDate(oneTierFileNodes);

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
            var input = new Dictionary<String, ObservableCollection<CFileSystemNode>>(oneTierFileNodes);

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
            var input = new Dictionary<String, ObservableCollection<CFileSystemNode>>(oneTierFileNodes);

            Dictionary<String, ObservableCollection<CFileSystemNode>> result = await 
                ArrangeController.SortByName(input);
            Assert.That(result[String.Empty].First().Entity.Name, Is.EqualTo("FolderA"));
            Assert.That(result[String.Empty].Last().Entity.Name, Is.EqualTo("wallhaven-p2l1vj.jpg"));
        }


        [Test]
        public void FilterStorageEntitiesByName()
        {
            Dictionary<String, ObservableCollection<IAccessWrapperUiModel>> result =
                ArrangeController.FilterByName(oneTierEntities, "*.txt", "*.jpg");

            Dictionary<String, ObservableCollection<IAccessWrapperUiModel>> resultViaUnion =
                ArrangeController.UnionFilterByName(oneTierEntities, "*.txt", "*.jpg");

            List<String> names = new List<string>();
            foreach (var accessUiModel in result[String.Empty])
            {
                names.Add(accessUiModel.Entity.Name);
            }

            Assert.That(names.Count, Is.EqualTo(4));
            Assert.That(names, Has.All.EndsWith(".txt").Or.EndsWith(".jpg"));

            names.Clear();
            foreach (var accessUiModel in resultViaUnion[String.Empty])
            {
                names.Add(accessUiModel.Entity.Name);
            }

            Assert.That(names.Count, Is.EqualTo(4));
            Assert.That(names, Has.All.EndsWith(".txt").Or.EndsWith(".jpg"));
        }

        [Test]
        public void ConstructFsModel()
        {
            CFileSystemNode root =
                ArrangeController.BuildFsModel(allTiersEntities);

            Assert.That(root.Children.Count, Is.EqualTo(12));

            var names = root.Children.Select(child => child.Entity.Name);
            Assert.That(names, Is.EquivalentTo(new String[]
            {
                "NewDir", "NewDir2", "FolderA", "FolderB", "FolderC", "FolderX",
                "new.txt", "icons8-nonya-kueh-96.ico", "SOLID.pdf", "sun-gorod-arkh.jpg", 
                "wallhaven-p2l1vj.jpg", "wallhaven-4yod9x.jpg"
            }));

            Assert.That(root.Children
                    .Single(node => node.Entity.Name.Equals("NewDir2"))
                    .Children.Single().Entity.Name, 
                Is.EqualTo("Advanced MVVM.pdf"));

            Assert.That(root.Children
                    .Single(node => node.Entity.Name.Equals("FolderB"))
                    .Children
                    .Single(node => node.Entity.Name.Equals("FolderM"))
                    .Children.Single().Entity.Name, 
                Is.EqualTo("rufus-3.6p.exe"));

            var testChildren = root.Children
                .Single(node => node.Entity.Name.Equals("FolderX"))
                .Children
                .Select(n => n.Entity.Name);
            
            Assert.That(testChildren, Is.EquivalentTo(new String[]
            {
                "FolderN", "Consensus.pdf"
            }));

        }

    }
}
