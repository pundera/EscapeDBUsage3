using EscapeDBUsage.UIClasses;
using EscapeDBUsage.UIClasses.OtherViews;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.Helpers
{
    public static class RefreshHelper
    {
        public static void RefreshTables(IEventAggregator evAgg, NodeBase root, ref ObservableCollection<NodeDbTableToExcel> nodes)
        {
            var newRoot = new NodeDbTableRoot(evAgg)
            {
                Nodes = new ObservableCollection<NodeDbTableToExcel>()
            };

            nodes = newRoot.Nodes;

            List<NodeBase> allTables = new List<NodeBase>();
            GetTables<NodeBase>(root, node => node.GetNodes(), ref allTables);

            var distinctTables = allTables.GroupBy(g => new { g.Name }).Select(g => g.First()).ToList();
            var orderedTables = distinctTables.OrderBy((x) => x.Name).ToList();

            nodes = new ObservableCollection<NodeDbTableToExcel>(orderedTables.Select((x) => new NodeDbTableToExcel(evAgg) { Name = x.Name, Description = x.Description, Nodes = new ObservableCollection<NodeDbTableToExcelToTab>() }).ToList());

            var allExcels = (root as NodeRoot).Nodes;
             
            foreach (var node in nodes) // TABLES
            {
                foreach (var table in allTables) // ALLTABLES
                {
                    foreach (var rootEx in allExcels)
                    {
                        if (!node.Nodes.Any(x => x.Name.Equals(rootEx.Name)))
                        {
                            var newExcel = new NodeDbTableToExcelToTab(evAgg) { Name = rootEx.Name, Description = rootEx.Description };
                            newExcel.Nodes = new ObservableCollection<NodeTab>(rootEx.Nodes.ToList().Where(x => x.Nodes != null && x.Nodes.Any(y => y.Name.Equals(node.Name))).ToList());
                            node.Nodes.Add(newExcel);
                        }
                    }
                }
            }
            
        }

        public static void RefreshTableColumns(IEventAggregator evAgg, NodeBase root, ref ObservableCollection<NodeDbTableColumnsToExcel> nodes)
        {
            var newRoot = new NodeDbTableColumnRoot(evAgg)
            {
                Nodes = new ObservableCollection<NodeDbTableColumnsToExcel>()
            };

            nodes = newRoot.Nodes;

            List<NodeBase> allColumns = new List<NodeBase>();
            GetColumns<NodeBase>(root, node => node.GetNodes(), ref allColumns);

            var distinctTableColumns = allColumns.GroupBy(g => new { ColumnName = (g as NodeDbColumn).Name, TableName = (g as NodeDbColumn).NodeDbTable.Name }).Select(g => g.First()).ToList();
            var orderedTableColumns = distinctTableColumns.OrderBy(
                (x) => 
                string.Format("{0} - {1}", (x as NodeDbColumn).NodeDbTable.Name, (x as NodeDbColumn).Name
                )
            ).ToList();

            nodes = new ObservableCollection<NodeDbTableColumnsToExcel>(orderedTableColumns.Select((x) => new NodeDbTableColumnsToExcel(evAgg) {
                Name = string.Format("{0} - {1}", (x as NodeDbColumn).NodeDbTable.Name, (x as NodeDbColumn).Name),
                Description = x.Description,
                Nodes = new ObservableCollection<NodeDbTableColumnsToExcelToTab>() }).ToList());

            var allExcels = (root as NodeRoot).Nodes;

            foreach (var node in nodes) // COLUMNS
            {
                foreach (var table in allColumns) // ALLCOLUMNS
                {
                    foreach (var rootEx in allExcels)
                    {
                        if (!node.Nodes.Any(x => x.Name.Equals(rootEx.Name)))
                        {
                            var newExcel = new NodeDbTableColumnsToExcelToTab(evAgg) { Name = rootEx.Name, Description = rootEx.Description };
                            newExcel.Nodes = new ObservableCollection<NodeTab>(
                                rootEx.Nodes.ToList()
                                    .Where(x => x.Nodes.Any(y =>
                                        y.Nodes.Any(z =>
                                            node.Name == string.Format("{0} - {1}", z.NodeDbTable.Name, z.Name)
                                            )
                                            )
                                    ).ToList());
                            node.Nodes.Add(newExcel);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the collection of leafs (items that have no children) from a hierarchical collection
        /// </summary>
        /// <typeparam name="T">The collection type</typeparam>
        /// <param name="source">The sourceitem of the collection</param>
        /// <param name="getChildren">A method that returns the children of an item</param>
        /// <returns>The collection of leafs</returns>
        private static IEnumerable<T> GetLeafs<T>(T source, Func<T, IEnumerable<T>> getChildren)
        {
            var list = getChildren(source);
            if (list == null || !list.Any())
            {
                yield return source;
            }
            else
            {
                foreach (var child in getChildren(source))
                {
                    foreach (var subchild in GetLeafs(child, getChildren))
                    {
                        yield return subchild;
                    }
                }
            }
        }

        private static void GetTables<T>(T source, Func<T, IEnumerable<T>> getChildren, ref List<T> result)
        {
            if (result == null) result = new List<T>();

            if (source==null)
            {
                return;
            }

            if (source is NodeDbTable)
            {
                result.Add(source);
                return;
            }
            else
            {
                foreach (var n in getChildren(source)) GetTables<T>(n, getChildren, ref result);
            }
        }

        private static void GetColumns<T>(T source, Func<T, IEnumerable<T>> getChildren, ref List<T> result)
        {
            if (result == null) result = new List<T>();


            if (source == null)
            {
                return;
            }

            if (source is NodeDbColumn)
            {
                result.Add(source);
                return;
            }
            else
            {
                foreach (var n in getChildren(source)) GetColumns<T>(n, getChildren, ref result);
            }
        }
    }

}
