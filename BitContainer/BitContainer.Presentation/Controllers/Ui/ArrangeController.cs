using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels;
using BitContainer.Presentation.ViewModels.Nodes;

namespace BitContainer.Presentation.Controllers.Ui
{
    public static class ArrangeController
    {
        public static Task<Dictionary<String, ObservableCollection<CFileSystemNode>>>
            GroupByCreationDate(Dictionary<String, ObservableCollection<CFileSystemNode>> entities)
        {
            return Task.Run(() =>
            {
                var flatSequence = entities
                    .SelectMany(group => group.Value)
                    .OrderByDescending(entity => entity.CreatedDate);

                return flatSequence
                    .GroupBy(entity => entity.CreatedDate)
                    .ToDictionary(
                        group => group.Key.ToLongDateString(), 
                        group => new ObservableCollection<CFileSystemNode>(group));
            });
        }

        public static Task<Dictionary<String, ObservableCollection<CFileSystemNode>>>
            DisableGroupping(Dictionary<String, ObservableCollection<CFileSystemNode>> entities)
        {
            return Task.Run(() =>
            {
                var flatSequence = entities
                    .SelectMany(group => group.Value)
                    .OrderByDescending(entity => entity.CreatedDate);
                var dict = new Dictionary<String, ObservableCollection<CFileSystemNode>>()
                {
                    [String.Empty] = new ObservableCollection<CFileSystemNode>(flatSequence)
                };
                return dict;
            });
        }

        public static Task<Dictionary<String, ObservableCollection<CFileSystemNode>>>
            SortByCreationTime(Dictionary<String, ObservableCollection<CFileSystemNode>> nodes)
        {
            return Task.Run(() =>
            {
                var dict = new Dictionary<String, ObservableCollection<CFileSystemNode>>();
                var keys = new List<String>(nodes.Keys);

                foreach (var k in keys)
                {
                    var ordered = nodes[k].OrderByDescending(node => node.CreatedTime);
                    dict[k] = new ObservableCollection<CFileSystemNode>(ordered);
                }

                return dict;
            });
        }

        public static Task<Dictionary<String, ObservableCollection<CFileSystemNode>>>
            SortByName(Dictionary<String, ObservableCollection<CFileSystemNode>> entities)
        {
            return Task.Run(() =>
            {
                var dict = new Dictionary<String, ObservableCollection<CFileSystemNode>>();
                var keys = new List<String>(entities.Keys);

                foreach (var k in keys)
                {
                    var ordered = entities[k].OrderBy(entity => entity.Entity.Name);
                    dict[k] = new ObservableCollection<CFileSystemNode>(ordered);
                }

                return dict;
            });
        }

        public static Dictionary<String, ObservableCollection<IAccessWrapperUiModel>>
            FilterByName(Dictionary<String, ObservableCollection<IAccessWrapperUiModel>> accessWrappers,
                params String[] patterns)
        {
            var regexs = new List<String>();
            foreach (var pattern in patterns)
            {
                String escaped = Regex.Escape(pattern);
                regexs.Add(escaped.Replace("\\*", ".*"));
            }
            
            var result = new Dictionary<String, ObservableCollection<IAccessWrapperUiModel>>(accessWrappers);

            var keys = new List<String>(accessWrappers.Keys);

            foreach (var k in keys)
            {
                var filtered = accessWrappers[k].Where(wrapper =>
                    regexs.Any(pattern => Regex.IsMatch(wrapper.Entity.Name, pattern)));
                result[k] = new ObservableCollection<IAccessWrapperUiModel>(filtered);
            }

            return result;
        }
    }
}
