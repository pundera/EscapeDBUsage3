using EscapeDBUsage.Enums;
using EscapeDBUsage.Events;
using EscapeDBUsage.Properties;
using EscapeDBUsage.UIClasses;
using EscapeDBUsage.UIClasses.Path;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.ViewModels
{
    public class PathViewModel: BindableBase
    {
        public PathViewModel(IEventAggregator eventAggregator, MainViewModel mainViewModel)
        {
            evAgg = eventAggregator;

            this.mainViewModel = mainViewModel;

            Root = new SelectedNodePathViewModel(evAgg, mainViewModel.Root, this, mainViewModel, NodeType.Root);
            Excel = new SelectedNodePathViewModel(evAgg, null, this, mainViewModel, NodeType.Excel) { Parent = root };
            Tab = new SelectedNodePathViewModel(evAgg, null, this, mainViewModel, NodeType.Tab) { Parent = excel };
            Table = new SelectedNodePathViewModel(evAgg, null, this, mainViewModel, NodeType.Table) { Parent = tab };
            Column = new SelectedNodePathViewModel(evAgg, null, this, mainViewModel, NodeType.Column) { Parent = table };

            evAgg.GetEvent<SelectedInMainChangedEvent>().Subscribe(PathChangedSubscribtion());
            evAgg.GetEvent<SelectedInPathChangedEvent>().Subscribe(PathChangedInPathSubscribtion());

            evAgg.GetEvent<PathViewAddedEvent>().Publish();
        }

        private bool isVisible = false;
        public bool IsVisible { get { return isVisible; } set { SetProperty(ref isVisible, value); } }

        private MainViewModel mainViewModel;

        private int countOfCycles = 0;

        public Action<NodeBase> PathChangedInPathSubscribtion()
        {
            return (node) =>
            {
                countOfCycles++;

                if (countOfCycles > 1)
                {
                    countOfCycles = 0;
                    return;
                }

                try
                {

                    //evAgg.GetEvent<SelectedInMainChangedEvent>().Unsubscribe(PathChangedSubscribtion());
                    evAgg.GetEvent<SelectedInPathChangedEvent>().Unsubscribe(PathChangedInPathSubscribtion());


                    //node.IsSelected = true;

                    //if (node is NodeRoot)
                    //{
                    //    Root = new SelectedNodePathViewModel(evAgg, node, this, mainViewModel, NodeType.Root) { Parent = null };
                    //    SetSelectedNodeProperty(node, Root, NodeType.Root, NodeType.Excel, true);
                    //}

                    //if (node is NodeExcel)
                    //{
                    //    Root = new SelectedNodePathViewModel(evAgg, node.GetParent(), this, mainViewModel, NodeType.Root) { Parent = null };
                    //    Excel = new SelectedNodePathViewModel(evAgg, node, this, mainViewModel, NodeType.Excel) { Parent = Root };

                    //    SetSelectedNodeProperty(node, Excel, NodeType.Excel, NodeType.Tab, true);
                    //}

                    //if (node is NodeTab)
                    //{
                    //    Root = new SelectedNodePathViewModel(evAgg, node.GetParent().GetParent(), this, mainViewModel, NodeType.Root) { Parent = null };
                    //    Excel = new SelectedNodePathViewModel(evAgg, node.GetParent(), this, mainViewModel, NodeType.Excel) { Parent = Root };
                    //    Tab = new SelectedNodePathViewModel(evAgg, node, this, mainViewModel, NodeType.Tab) { Parent = Excel };

                    //    SetSelectedNodeProperty(node, Tab, NodeType.Tab, NodeType.Table, true);
                    //}


                    //if (node is NodeDbTable)
                    //{
                    //    Root = new SelectedNodePathViewModel(evAgg, node.GetParent().GetParent().GetParent(), this, mainViewModel, NodeType.Root) { Parent = null };
                    //    Excel = new SelectedNodePathViewModel(evAgg, node.GetParent().GetParent(), this, mainViewModel, NodeType.Excel) { Parent = Root };
                    //    Tab = new SelectedNodePathViewModel(evAgg, node.GetParent(), this, mainViewModel, NodeType.Tab) { Parent = Excel };
                    //    Table = new SelectedNodePathViewModel(evAgg, node, this, mainViewModel, NodeType.Table) { Parent = Tab };

                    //    SetSelectedNodeProperty(node, Table, NodeType.Table, NodeType.Column, true);
                    //}

                    //if (node is NodeDbColumn)
                    //{
                    //    Root = new SelectedNodePathViewModel(evAgg, node.GetParent().GetParent().GetParent().GetParent(), this, mainViewModel, NodeType.Root) { Parent = null };
                    //    Excel = new SelectedNodePathViewModel(evAgg, node.GetParent().GetParent().GetParent(), this, mainViewModel, NodeType.Excel) { Parent = Root };
                    //    Tab = new SelectedNodePathViewModel(evAgg, node.GetParent().GetParent(), this, mainViewModel, NodeType.Tab) { Parent = Excel };
                    //    Table = new SelectedNodePathViewModel(evAgg, node.GetParent(), this, mainViewModel, NodeType.Table) { Parent = Tab };
                    //    Column = new SelectedNodePathViewModel(evAgg, node, this, mainViewModel, NodeType.Column) { Parent = Table };

                    //    SetSelectedNodeProperty(node, Column, NodeType.Column, NodeType.Undefined, true);
                    //}

                }
                finally
                {
                    //evAgg.GetEvent<SelectedInMainChangedEvent>().Subscribe(PathChangedSubscribtion());
                    evAgg.GetEvent<SelectedInPathChangedEvent>().Subscribe(PathChangedInPathSubscribtion());

                    countOfCycles = 0;
                    IsVisible = true;
                }
            };
        }


        public Action<NodeBase> PathChangedSubscribtion()
        {
            return (node) =>
            {
                countOfCycles++;

                if (countOfCycles>1)
                {
                    countOfCycles = 0;
                    return;
                }

                try
                {

                    evAgg.GetEvent<SelectedInMainChangedEvent>().Unsubscribe(PathChangedSubscribtion());
                    evAgg.GetEvent<SelectedInPathChangedEvent>().Unsubscribe(PathChangedInPathSubscribtion());


                    var guid = node.Guid;

                    if (node is NodeRoot)
                    {
                        Root = new SelectedNodePathViewModel(evAgg, node, this, mainViewModel, NodeType.Root) { Parent = null };
                        SetSelectedNodeProperty(node, Root, NodeType.Root, NodeType.Excel, guid);
                    }

                    if (node is NodeExcel)
                    {
                        Root = new SelectedNodePathViewModel(evAgg, node.GetParent(), this, mainViewModel, NodeType.Root) { Parent = null };
                        Excel = new SelectedNodePathViewModel(evAgg, node, this, mainViewModel, NodeType.Excel) { Parent = Root };

                        SetSelectedNodeProperty(node, Excel, NodeType.Excel, NodeType.Tab, guid);
                    }

                    if (node is NodeTab)
                    {
                        Root = new SelectedNodePathViewModel(evAgg, node.GetParent().GetParent(), this, mainViewModel, NodeType.Root) { Parent = null };
                        Excel = new SelectedNodePathViewModel(evAgg, node.GetParent(), this, mainViewModel, NodeType.Excel) { Parent = Root };
                        Tab = new SelectedNodePathViewModel(evAgg, node, this, mainViewModel, NodeType.Tab) { Parent = Excel };

                        SetSelectedNodeProperty(node, Tab, NodeType.Tab, NodeType.Table, guid);
                    }


                    if (node is NodeDbTable)
                    {
                        Root = new SelectedNodePathViewModel(evAgg, node.GetParent().GetParent().GetParent(), this, mainViewModel, NodeType.Root) { Parent = null };
                        Excel = new SelectedNodePathViewModel(evAgg, node.GetParent().GetParent(), this, mainViewModel, NodeType.Excel) { Parent = Root };
                        Tab = new SelectedNodePathViewModel(evAgg, node.GetParent(), this, mainViewModel, NodeType.Tab) { Parent = Excel };
                        Table = new SelectedNodePathViewModel(evAgg, node, this, mainViewModel, NodeType.Table) { Parent = Tab };

                        SetSelectedNodeProperty(node, Table, NodeType.Table, NodeType.Column, guid);
                    }

                    if (node is NodeDbColumn)
                    {
                        Root = new SelectedNodePathViewModel(evAgg, node.GetParent().GetParent().GetParent().GetParent(), this, mainViewModel, NodeType.Root) { Parent = null };
                        Excel = new SelectedNodePathViewModel(evAgg, node.GetParent().GetParent().GetParent(), this, mainViewModel, NodeType.Excel) { Parent = Root };
                        Tab = new SelectedNodePathViewModel(evAgg, node.GetParent().GetParent(), this, mainViewModel, NodeType.Tab) { Parent = Excel };
                        Table = new SelectedNodePathViewModel(evAgg, node.GetParent(), this, mainViewModel, NodeType.Table) { Parent = Tab };
                        Column = new SelectedNodePathViewModel(evAgg, node, this, mainViewModel, NodeType.Column) { Parent = Table };

                        SetSelectedNodeProperty(node, Column, NodeType.Column, NodeType.Undefined, guid);
                    }

                }
                finally
                {
                    evAgg.GetEvent<SelectedInMainChangedEvent>().Subscribe(PathChangedSubscribtion());
                    evAgg.GetEvent<SelectedInPathChangedEvent>().Subscribe(PathChangedInPathSubscribtion());

                    countOfCycles = 0;
                    IsVisible = true;
                }
            };
        }

        private void SetSelectedNodeProperty(NodeBase node, SelectedNodePathViewModel selectedNode, NodeType nodeType, NodeType nodeItemsType, Guid guid, bool isFromPath = false)
        {
            selectedNode.Node = node;
            var img = "root";
            var imgItems = "excel";
            selectedNode.ImageSource = string.Format(@"pack://application:,,,/Resources/{0}.png", img);
            selectedNode.Name = node.Name;        

            SetSelectedItem(Root.Node as NodeRoot, node, selectedNode, nodeType, nodeItemsType, guid, isFromPath);

            Root.IsVisible = false;
            if (Excel!=null) Excel.IsVisible = false;
            if (Tab != null) Tab.IsVisible = false;
            if (Table != null) Table.IsVisible = false;
            if (Column != null) Column.IsVisible = false;

            if (selectedNode.Parent != null)
            {
                selectedNode.Parent.IsVisible = true;
                if (selectedNode.Parent.Parent != null)
                {
                    selectedNode.Parent.Parent.IsVisible = true;
                    if (selectedNode.Parent.Parent.Parent != null)
                    {
                        selectedNode.Parent.Parent.Parent.IsVisible = true;
                        if (selectedNode.Parent.Parent.Parent.Parent != null) selectedNode.Parent.Parent.Parent.Parent.IsVisible = true;
                    }
                }
            }

            selectedNode.IsVisible = true;
            Column.IsVisible = false;

            //selectedNode.Node.EventAggregator.GetEvent<SelectionStructureChangedEvent>().Unsubscribe(PathChangedSubscribtion());
            selectedNode.Node.IsSelected = true;
            //selectedNode.Node.EventAggregator.GetEvent<SelectionStructureChangedEvent>().Subscribe(PathChangedSubscribtion());
        }

        private void SetSelectedItem(NodeRoot root, NodeBase node, SelectedNodePathViewModel pathItem, NodeType nodeType, NodeType nodeItemsType, Guid guid, bool isFromPath)
        {

            if (nodeType != NodeType.Column)
            {
                pathItem.Items = new ObservableCollection<PathItem>(node.GetNodes().Select(x => new PathItem()
                {
                    Guid = x.Guid,
                    Name = x.Name
                }));
            }

            if (pathItem.Parent != null)
            {
                pathItem.Parent.Items = new ObservableCollection<PathItem>(node.GetParent().GetNodes().Select(x => new PathItem()
                {
                    Guid = x.Guid,
                    Name = x.Name
                }));
            }

            if (pathItem.Parent != null && pathItem.Parent.Parent != null)
            {
                pathItem.Parent.Parent.Items = new ObservableCollection<PathItem>(node.GetParent().GetParent().GetNodes().Select(x => new PathItem()
                {
                    Guid = x.Guid,
                    Name = x.Name
                }));
            }

            if (pathItem.Parent != null && pathItem.Parent.Parent != null && pathItem.Parent.Parent.Parent != null)
            {
                pathItem.Parent.Parent.Parent.Items = new ObservableCollection<PathItem>(node.GetParent().GetParent().GetParent().GetNodes().Select(x => new PathItem()
                {
                    Guid = x.Guid,
                    Name = x.Name
                }));
            }

            if (pathItem.Parent != null && pathItem.Parent.Parent != null && pathItem.Parent.Parent.Parent != null && pathItem.Parent.Parent.Parent.Parent != null)
            {
                pathItem.Parent.Parent.Parent.Parent.Items = new ObservableCollection<PathItem>(node.GetParent().GetParent().GetParent().GetParent().GetNodes().Select(x => new PathItem()
                {
                    Guid = x.Guid,
                    Name = x.Name
                }));
            }


            if (nodeType == NodeType.Root)
            {
                //Root
                //pathItem.SelectedItem = pathItem.Items.FirstOrDefault();
            }

            if (nodeType == NodeType.Excel)
            {
                // Root -> 
                //pathItem.Parent.Node = mainViewModel.Root;
                pathItem.Parent.SelectedItem = pathItem.Parent.Items.FirstOrDefault(x => x.Guid.Equals(guid));
            }

            if (nodeType == NodeType.Tab)
            {
                // Root -> 
                pathItem.Parent.Parent.SelectedItem = pathItem.Parent.Parent.Items.FirstOrDefault(x => x.Guid.Equals(node.GetParent().Guid));
                // Excel -> 
                pathItem.Parent.SelectedItem = pathItem.Parent.Items.FirstOrDefault(x => x.Guid.Equals(guid));
            }

            if (nodeType == NodeType.Table)
            {
                // Root -> 
                var excelNode = node.GetParent().GetParent();
                pathItem.Parent.Parent.Parent.SelectedItem = pathItem.Parent.Parent.Parent.Items.FirstOrDefault(x => x.Guid.Equals(excelNode.Guid));
                // Excel -> 
                pathItem.Parent.Parent.SelectedItem = pathItem.Parent.Parent.Items.FirstOrDefault(x => x.Guid.Equals(node.GetParent().Guid));
                // Tab -> 
                pathItem.Parent.SelectedItem = pathItem.Parent.Items.FirstOrDefault(x => x.Guid.Equals(guid));
            }

            if (nodeType == NodeType.Column)
            {
                // Root -> 
                pathItem.Parent.Parent.Parent.Parent.SelectedItem = pathItem.Parent.Parent.Parent.Parent.Items.FirstOrDefault(x => x.Guid.Equals(node.GetParent().GetParent().GetParent().Guid));
                // Excel -> 
                pathItem.Parent.Parent.Parent.SelectedItem = pathItem.Parent.Parent.Parent.Items.FirstOrDefault(x => x.Guid.Equals(node.GetParent().GetParent().Guid));
                // Tab -> 
                pathItem.Parent.Parent.SelectedItem = pathItem.Parent.Parent.Items.FirstOrDefault(x => x.Guid.Equals(node.GetParent().Guid));
                // Table -> 
                pathItem.Parent.SelectedItem = pathItem.Parent.Items.FirstOrDefault(x => x.Guid.Equals(guid));
            }

        }

        private IEventAggregator evAgg;


        private SelectedNodePathViewModel root;
        public SelectedNodePathViewModel Root
        {
            get { return root; }
            set { SetProperty(ref root, value); }
        }

        private SelectedNodePathViewModel excel;
        public SelectedNodePathViewModel Excel
        {
            get { return excel; }
            set { SetProperty(ref excel, value); }
        }

        private SelectedNodePathViewModel tab;
        public SelectedNodePathViewModel Tab
        {
            get { return tab; }
            set { SetProperty(ref tab, value); }
        }

        private SelectedNodePathViewModel table;
        public SelectedNodePathViewModel Table
        {
            get { return table; }
            set { SetProperty(ref table, value); }
        }

        private SelectedNodePathViewModel column;
        public SelectedNodePathViewModel Column
        {
            get { return column; }
            set { SetProperty(ref column, value); }
        }
    }
}
