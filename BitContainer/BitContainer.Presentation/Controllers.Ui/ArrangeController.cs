using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels;
using BitContainer.Presentation.ViewModels.Enums;
using BitContainer.Presentation.ViewModels.Nodes;

namespace BitContainer.Presentation.Controllers.Ui
{
    public class ArrangeController
    {
        private readonly Dictionary<GroupType, Func<FileSystemNode, string>> _groupKeys;
        private readonly Dictionary<SortType, Func<FileSystemNode, string>> _sortKeys;

        public Dictionary<String, ObservableCollection<FileSystemNode>> Arrangement { get; private set; }
        public Boolean IsEmptyArrangement => Arrangement.Count == 1 && Arrangement[Arrangement.Keys.First()].Count == 0;
        private GroupType _lastGroupType;
        private SortType _lastSortType;

        public ArrangeController()
            : this(new List<FileSystemNode>(), GroupType.None, SortType.Created)
        {

        }

        public ArrangeController(IEnumerable<FileSystemNode> nodes, GroupType groupType, SortType sortType)
        {
            _groupKeys = new Dictionary<GroupType, Func<FileSystemNode, string>>
            {
                [GroupType.None] = (node) => string.Empty,
                [GroupType.Created] = (node) => node.CreatedDate.ToLongDateString()
            };
            _sortKeys = new Dictionary<SortType, Func<FileSystemNode, string>>
            {
                [SortType.Created] = (node) => node.CreatedDateTime.Ticks.ToString(),
                [SortType.Name] = (node) => node.Name
            };

            var dict = new Dictionary<String, ObservableCollection<FileSystemNode>>
            {
                [String.Empty] = new ObservableCollection<FileSystemNode>(nodes)
            };

            Arrangement = dict;

            _lastGroupType = groupType;
            _lastSortType = sortType;

            Group(groupType);
        }
        
        public Task<Dictionary<String, ObservableCollection<FileSystemNode>>> Reset(IEnumerable<FileSystemNode> nodes, GroupType groupType, SortType sortType)
        {
            var dict = new Dictionary<String, ObservableCollection<FileSystemNode>>
            {
                [String.Empty] = new ObservableCollection<FileSystemNode>(nodes)
            };

           Arrangement = dict;
           return Arrange(groupType, sortType);
        }

        public Task<Dictionary<String, ObservableCollection<FileSystemNode>>> Arrange(GroupType groupType, SortType sortType)
        {
            _lastGroupType = groupType;
            _lastSortType = sortType;
            return Task.Run(() => Group(groupType));
        }

        public Task<Dictionary<String, ObservableCollection<FileSystemNode>>> Arrange(GroupType groupType)
        {
            _lastGroupType = groupType;
            return Task.Run(() => Group(groupType));
        }

        public Task<Dictionary<String, ObservableCollection<FileSystemNode>>> Arrange(SortType sortType)
        {
            _lastSortType = sortType;
            return Task.Run(() => Sort(sortType));
        }

        public void AddToArrangement(FileSystemNode node)
        {
            String key = _groupKeys[_lastGroupType](node);
            Arrangement[key].Add(node);
            Sort(key, _lastSortType);
        }

        public void RemoveFromArrangement(FileSystemNode node)
        {
            String key = _groupKeys[_lastGroupType](node);
            if (Arrangement.ContainsKey(key))
                Arrangement[key].Remove(node);
        }

        private Dictionary<String, ObservableCollection<FileSystemNode>> Group(GroupType groupType)
        {
            if (IsEmptyArrangement) return Arrangement;

            var flatSequence = Arrangement.SelectMany(group => group.Value);
            Arrangement = flatSequence
                .GroupBy(entity => _groupKeys[groupType](entity))
                .ToDictionary(
                    group => group.Key, 
                    group => new ObservableCollection<FileSystemNode>(group));
            
            Sort(_lastSortType);

            return Arrangement;
        }

        private Dictionary<String, ObservableCollection<FileSystemNode>> Sort(SortType sortType)
        {
            var dict = new Dictionary<String, ObservableCollection<FileSystemNode>>();
            var keys = new List<String>(Arrangement.Keys);

            foreach (var k in keys)
            {
                dict[k] = Sort(k, sortType);
            }

            Arrangement = dict;

            return Arrangement;
        }

        private ObservableCollection<FileSystemNode> Sort(String key, SortType sortType)
        {
            var ordered = Arrangement[key].OrderByDescending(node => _sortKeys[sortType](node)).ToList();
            return new ObservableCollection<FileSystemNode>(ordered);
        }

    }
}
