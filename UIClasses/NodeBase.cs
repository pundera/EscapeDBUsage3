using EscapeDBUsage.Events;
using EscapeDBUsage.UIClasses.OtherViews;
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

namespace EscapeDBUsage.UIClasses
{
    public class NodeBase: BindableBase
    {
        public NodeBase(IEventAggregator evAgg)
        {
            EventAggregator = evAgg;

            Remove = new DelegateCommand(DoRemove);
        
            Up = new DelegateCommand(DoUp);
            Down = new DelegateCommand(DoDown);
        }


        private bool areDescsShown = false;
        public bool AreDescsShown
        {
            get
            {
                return areDescsShown;
            }
            set
            {
                SetProperty(ref areDescsShown, value);
            }
        }

        private void DoDown()
        {
            if (this is NodeExcel)
            {
                var nodes = (this as NodeExcel).Root.Nodes;
                var ix = nodes.IndexOf(this as NodeExcel);
                if (ix == nodes.Count - 1) return;
                nodes.Remove(this as NodeExcel);
                nodes.Insert(ix + 1, this as NodeExcel);
            }
            if (this is NodeTab)
            {
                var nodes = (this as NodeTab).NodeExcel.Nodes;
                var ix = nodes.IndexOf(this as NodeTab);
                if (ix == nodes.Count-1) return;
                nodes.Remove(this as NodeTab);
                nodes.Insert(ix + 1, this as NodeTab);
            }
            if (this is NodeDbTable)
            {
                var nodes = (this as NodeDbTable).NodeTab.Nodes;
                var ix = nodes.IndexOf(this as NodeDbTable);
                if (ix == nodes.Count-1) return;
                nodes.Remove(this as NodeDbTable);
                nodes.Insert(ix + 1, this as NodeDbTable);
            }
            if (this is NodeDbColumn)
            {
                var nodes = (this as NodeDbColumn).NodeDbTable.Nodes;
                var ix = nodes.IndexOf(this as NodeDbColumn);
                if (ix == nodes.Count-1) return;
                nodes.Remove(this as NodeDbColumn);
                nodes.Insert(ix + 1, this as NodeDbColumn);
            }

            SelectedNode = this;

        }

        private void DoUp()
        {
            if (this is NodeExcel)
            {
                var nodes = (this as NodeExcel).Root.Nodes;
                var ix = nodes.IndexOf(this as NodeExcel);
                if (ix == 0) return;
                nodes.Remove(this as NodeExcel);
                nodes.Insert(ix - 1, this as NodeExcel);
            }
            if (this is NodeTab)
            {
                var nodes = (this as NodeTab).NodeExcel.Nodes;
                var ix = nodes.IndexOf(this as NodeTab);
                if (ix == 0) return;
                nodes.Remove(this as NodeTab);
                nodes.Insert(ix - 1, this as NodeTab);
            }
            if (this is NodeDbTable)
            {
                var nodes = (this as NodeDbTable).NodeTab.Nodes;
                var ix = nodes.IndexOf(this as NodeDbTable);
                if (ix == 0) return;
                nodes.Remove(this as NodeDbTable);
                nodes.Insert(ix - 1, this as NodeDbTable);
            }
            if (this is NodeDbColumn)
            {
                var nodes = (this as NodeDbColumn).NodeDbTable.Nodes;
                var ix = nodes.IndexOf(this as NodeDbColumn);
                if (ix == 0) return;
                nodes.Remove(this as NodeDbColumn);
                nodes.Insert(ix - 1, this as NodeDbColumn);
            }

            SelectedNode = this;
        }

        public IEventAggregator EventAggregator { get; private set; }

        public ICommand Remove { get; private set; }

        public ICommand Up { get; private set; }
        public ICommand Down { get; private set; }

        private void DoRemove()
        {
            if (this is NodeExcel) (this as NodeExcel).Root.Nodes.Remove(this as NodeExcel);
            if (this is NodeTab) (this as NodeTab).NodeExcel.Nodes.Remove(this as NodeTab);
            if (this is NodeDbTable) (this as NodeDbTable).NodeTab.Nodes.Remove(this as NodeDbTable);
            if (this is NodeDbColumn) (this as NodeDbColumn).NodeDbTable.Nodes.Remove(this as NodeDbColumn);
        }


        private NodeBase selectedNode;
        public NodeBase SelectedNode
        {
            get { return selectedNode; }
            set
            {
                selectedNode = value;
                EventAggregator.GetEvent<EventSelectedChanged>().Publish(value);
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        private bool isExpanded = false;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set { SetProperty(ref isExpanded, value); }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                SetProperty(ref isSelected, value);
                SelectedNode = this;
            }
        }

        private bool isVisible = true;
        public bool IsVisible
        {
            get { return isVisible; }
            set { SetProperty(ref isVisible, value); }
        }

        public List<NodeBase> GetNodes()
        {
            if (this is NodeRoot) return (this as NodeRoot).Nodes.ToList<NodeBase>();
            if (this is NodeDbTableRoot) return (this as NodeDbTableRoot).Nodes.ToList<NodeBase>();
            if (this is NodeDbTableToExcel) return (this as NodeDbTableToExcel).Nodes.ToList<NodeBase>();

            if (this is NodeExcel) return (this as NodeExcel).Nodes.ToList<NodeBase>();
            if (this is NodeTab) return (this as NodeTab).Nodes.ToList<NodeBase>();
            if (this is NodeDbTable) return (this as NodeDbTable).Nodes.ToList<NodeBase>();
            if (this is NodeDbColumn) return null;
            return null;
        }

    }
}
