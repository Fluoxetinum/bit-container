using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels;

namespace BitContainer.Presentation.Controllers
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

        /// <summary>
        /// This method is excessive and it have performance and memory issues.
        /// It is primarily created for LINQ UNION demonstration inside project's domain.
        /// </summary>
        /// <param name="accessWrappers"></param>
        /// <param name="patterns"></param>
        /// <returns></returns>
        public static Dictionary<String, ObservableCollection<IAccessWrapperUiModel>>
            UnionFilterByName(Dictionary<String, ObservableCollection<IAccessWrapperUiModel>> accessWrappers,
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
                IEnumerable<IAccessWrapperUiModel> allPatternsFiltered = new List<IAccessWrapperUiModel>();
                foreach (var regex in regexs)
                {
                    var filtered = accessWrappers[k].Where(wrapper => Regex.IsMatch(wrapper.Entity.Name, regex));
                    allPatternsFiltered = allPatternsFiltered.Union(filtered);
                }
                result[k] = new ObservableCollection<IAccessWrapperUiModel>(allPatternsFiltered);
            }

            return result;
        }

        /// <summary>
        /// This method is excessive and it have performance and memory issues.
        /// It is primarily created for LINQ JOIN demonstration inside project's domain.
        /// </summary>
        /// <param name="accessWrappers"></param>
        /// <returns>Root of the file system model.</returns>
        public static CFileSystemNode BuildFsModel(List<IAccessWrapperUiModel> accessWrappers)
        {
            List<CFileSystemNode> nodes = (from e in accessWrappers
                    select new CFileSystemNode(parent:null, value:e)
                ).ToList();

            var nodesHierarchy = from n1 in nodes
                                 join n2 in nodes on n1.Entity.Id equals n2.Entity.ParentId
                                 into children
                                 select new 
                                 {
                                    Node = n1,
                                    NodeChildren = children 
                                 };

            foreach (var fileSystemNode in nodesHierarchy)
            {
                CFileSystemNode node = fileSystemNode.Node;
                CFileSystemNode parentNode =
                    nodes.SingleOrDefault(n => n.Entity.Id == fileSystemNode.Node.Entity.ParentId);
                node.Parent = parentNode;
                node.Children = new ObservableCollection<CFileSystemNode>(fileSystemNode.NodeChildren);
            }

            CFileSystemNode rootNode = new CFileSystemNode(parent: null, value: null);
            rootNode.Children = new ObservableCollection<CFileSystemNode>();

            foreach (var fileSystemNode in nodes)
            {
                if (fileSystemNode.Parent == null)
                {
                    fileSystemNode.Parent = rootNode;
                    rootNode.Children.Add(fileSystemNode);
                }
            }

            return rootNode;
        }

    }
}
