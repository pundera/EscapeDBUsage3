using EscapeDBUsage.Enums;
using EscapeDBUsage.Events;
using EscapeDBUsage.FlowStructures;
using EscapeDBUsage.UIClasses;
using EscapeDBUsage.UIClasses.Path;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace EscapeDBUsage.ViewModels
{
    public class SelectedNodePathViewModel: BindableBase
    {
        public SelectedNodePathViewModel(
            IEventAggregator eventAggregator, 
            NodeBase node, 
            PathViewModel viewModel, 
            MainViewModel mainViewModel,
            NodeType nodeType = NodeType.Undefined)
        {
            evAgg = eventAggregator;
            Node = node;
            NodeType = nodeType;
            pathViewModel = viewModel;
            this.mainViewModel = mainViewModel;

            IsVisible = false;

            switch (nodeType)
            {
                case NodeType.Root:
                    LabelName = "Excel:";
                    break;
                case NodeType.Excel:
                    LabelName = "Sheet:";
                    break;
                case NodeType.Sheet:
                    LabelName = "DB Table:";
                    break;
                case NodeType.Table:
                    LabelName = "DB Column:";
                    break;
                case NodeType.Column:
                    LabelName = " undefined ";
                    break;
            }
        }

        public string LabelName { get; private set; }

        public SubscriptionToken Token { get; set; }
        public Action<NodeBase> SubscriptionAction { get; set; }

        public NodeType NodeType { get; private set; }

        public SelectedNodePathViewModel Parent { get; set; }

        private PathViewModel pathViewModel;
        private MainViewModel mainViewModel;

        public NodeBase Node { get; set; }
        private IEventAggregator evAgg;

        private bool isRoot = false;
        public bool IsRoot
        {
            get { return isRoot; }
            set { SetProperty(ref isRoot, value); }
        }

        private bool isVisible = false;
        public bool IsVisible
        {
            get { return isVisible; }
            set { SetProperty(ref isVisible, value); }
        }

        private string imageSource;
        public string ImageSource
        {
            get { return imageSource; }
            set { SetProperty(ref imageSource, value); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private ObservableCollection<PathItem> items;
        public ObservableCollection<PathItem> Items
        {
            get { return items; }
            set { SetProperty(ref items, value); }
        }

        private PathItem selectedItem;
        public PathItem SelectedItem {
            get { return selectedItem; }
            set
            {
                if (value != null)
                {

                    SetProperty(ref selectedItem, value);

                    try
                    {
                        var n = Node.GetNodes().First(x => x.Guid.Equals(value.Guid));
                        n.IsSelected = true;
                    }
                    finally
                    {
                    }


                }
            }
        }

        public ICommand Select { get; private set; }
    }
}
