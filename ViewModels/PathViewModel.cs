﻿using EscapeDBUsage.Enums;
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

            SubscriptionTokenPathChanged = evAgg.GetEvent<SelectedInMainChangedEvent>().Subscribe(PathChangedSubscription());

            Root = new SelectedNodePathViewModel(evAgg, mainViewModel.Root, this, mainViewModel, NodeType.Root) { Token = SubscriptionTokenPathChanged, SubscriptionAction = PathChangedSubscription() };
            Excel = new SelectedNodePathViewModel(evAgg, null, this, mainViewModel, NodeType.Excel) { Parent = root, Token = SubscriptionTokenPathChanged, SubscriptionAction = PathChangedSubscription() };
            Tab = new SelectedNodePathViewModel(evAgg, null, this, mainViewModel, NodeType.Sheet) { Parent = excel, Token = SubscriptionTokenPathChanged, SubscriptionAction = PathChangedSubscription() };
            Table = new SelectedNodePathViewModel(evAgg, null, this, mainViewModel, NodeType.Table) { Parent = tab, Token = SubscriptionTokenPathChanged, SubscriptionAction = PathChangedSubscription() };
            Column = new SelectedNodePathViewModel(evAgg, null, this, mainViewModel, NodeType.Column) { Parent = table, Token = SubscriptionTokenPathChanged, SubscriptionAction = PathChangedSubscription() };


            evAgg.GetEvent<PathViewAddedEvent>().Publish();
        }
        
        public SubscriptionToken SubscriptionTokenPathChanged { get; set; }

        private bool isVisible = false;
        public bool IsVisible { get { return isVisible; } set { SetProperty(ref isVisible, value); } }

        private MainViewModel mainViewModel;

        private int countOfCycles = 0;

        public Action<NodeBase> PathChangedSubscription()
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

                    evAgg.GetEvent<SelectedInMainChangedEvent>().Unsubscribe(SubscriptionTokenPathChanged);


                    var guid = node.Guid;

                    if (node is NodeRoot)
                    {
                        Root.Node = node;
                        SetSelectedNodeProperty(node, Root, NodeType.Root, NodeType.Excel, guid);
                    }

                    if (node is NodeExcel)
                    {
                        Root.Node = node.GetParent();
                        Excel.Node = node;
                        SetSelectedNodeProperty(node, Excel, NodeType.Excel, NodeType.Sheet, guid);
                    }

                    if (node is NodeTab)
                    {
                        Root.Node = node.GetParent().GetParent();
                        Excel.Node = node.GetParent();
                        Tab.Node = node; 
                        SetSelectedNodeProperty(node, Tab, NodeType.Sheet, NodeType.Table, guid);
                    }


                    if (node is NodeDbTable)
                    {
                        Root.Node = node.GetParent().GetParent().GetParent();
                        Excel.Node = node.GetParent().GetParent();
                        Tab.Node = node.GetParent();
                        Table.Node = node;

                        SetSelectedNodeProperty(node, Table, NodeType.Table, NodeType.Column, guid);
                    }

                    if (node is NodeDbColumn)
                    {
                        Root.Node = node.GetParent().GetParent().GetParent().GetParent();
                        Excel.Node = node.GetParent().GetParent().GetParent();
                        Tab.Node = node.GetParent().GetParent();
                        Table.Node = node.GetParent();
                        Column.Node = node;

                        SetSelectedNodeProperty(node, Column, NodeType.Column, NodeType.Undefined, guid);
                    }

                }
                finally
                {
                    SubscriptionTokenPathChanged = evAgg.GetEvent<SelectedInMainChangedEvent>().Subscribe(PathChangedSubscription());

                    countOfCycles = 0;
                    IsVisible = true;
                }
            };
        }

        private void SetSelectedNodeProperty(NodeBase node, SelectedNodePathViewModel selectedNode, NodeType nodeType, NodeType nodeItemsType, Guid guid, bool isFromPath = false)
        {
            selectedNode.Node = node;
            var img = "root";
            var imgItems = "excel|tab|table|column".Split('|');
            switch (nodeType)
            {
                case NodeType.Root: img = "excel"; break;
                case NodeType.Excel: img = "tab"; break;
                case NodeType.Sheet: img = "table"; break;
                case NodeType.Table: img = "column"; break;
                //case NodeType.Column: img = "column"; break;
            }
            selectedNode.ImageSource = string.Format(@"pack://application:,,,/Images/{0}.png", img);
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

        }

        private void SetSelectedItem(NodeRoot root, NodeBase node, SelectedNodePathViewModel pathItem, NodeType nodeType, NodeType nodeItemsType, Guid guid, bool isFromPath)
        {

            if (nodeType != NodeType.Column)
            {
                pathItem.Items = new ObservableCollection<PathItem>(node.GetNodes().Select(x => new PathItem(x)
                {
                    Guid = (x as NodeBase).Guid,
                    Name = (x as NodeBase).Name
                }));
            }

            if (pathItem.Parent != null)
            {
                pathItem.Parent.Items = new ObservableCollection<PathItem>(node.GetParent().GetNodes().Select(x => new PathItem(x)
                {
                    Guid = (x as NodeBase).Guid,
                    Name = (x as NodeBase).Name
                }));
            }

            if (pathItem.Parent != null && pathItem.Parent.Parent != null)
            {
                pathItem.Parent.Parent.Items = new ObservableCollection<PathItem>(node.GetParent().GetParent().GetNodes().Select(x => new PathItem(x)
                {
                    Guid = (x as NodeBase).Guid,
                    Name = (x as NodeBase).Name
                }));
            }

            if (pathItem.Parent != null && pathItem.Parent.Parent != null && pathItem.Parent.Parent.Parent != null)
            {
                pathItem.Parent.Parent.Parent.Items = new ObservableCollection<PathItem>(node.GetParent().GetParent().GetParent().GetNodes().Select(x => new PathItem(x)
                {
                    Guid = (x as NodeBase).Guid,
                    Name = (x as NodeBase).Name
                }));
            }

            if (pathItem.Parent != null && pathItem.Parent.Parent != null && pathItem.Parent.Parent.Parent != null && pathItem.Parent.Parent.Parent.Parent != null)
            {
                pathItem.Parent.Parent.Parent.Parent.Items = new ObservableCollection<PathItem>(node.GetParent().GetParent().GetParent().GetParent().GetNodes().Select(x => new PathItem(x)
                {
                    Guid = (x as NodeBase).Guid,
                    Name = (x as NodeBase).Name
                }));
            }


            if (nodeType == NodeType.Root)
            {
                //Root
                var excel = pathItem.Items.FirstOrDefault();
                pathItem.SelectedItem = excel;
                //var nodes = pathItem.Node.GetNodes();
                //var excelNode = nodes.First(e => e.Guid.Equals(excel.Guid));
                //countOfCycles = 0;
                //PathChangedSubscription().Invoke(excelNode);
            }

            if (nodeType == NodeType.Excel)
            {
                pathItem.Parent.SelectedItem = pathItem.Parent.Items.FirstOrDefault(x => x.Guid.Equals(guid));
            }

            if (nodeType == NodeType.Sheet)
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
