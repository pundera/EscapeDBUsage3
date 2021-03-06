﻿using EscapeDBUsage.Interfaces;
using EscapeDBUsage.ViewModels;
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
    public class NodeDbTable : NodeBase, IFulltext
    {
        public NodeDbTable(IEventAggregator eventAggregator, NodeTab nodeTab, MainViewModel viewModel) : base(eventAggregator)
        {
            this.viewModel = viewModel;
            NodeTab = nodeTab;
            AddColumn = new DelegateCommand(DoAddColumn);
        }

        private MainViewModel viewModel;

        public ICommand AddColumn { get; private set; }

        private void DoAddColumn()
        {
            if (Nodes == null) Nodes = new ObservableCollection<IFulltext>();
            var c = new NodeDbColumn(EventAggregator, this, viewModel);
            Nodes.Insert(0, c);
            viewModel.SelectedDbColumn = c;
            c.IsSelected = true;
        }


        private NodeTab nodeTab;
        public NodeTab NodeTab
        {
            get { return nodeTab; }
            set { SetProperty(ref nodeTab, value); }
        }

        private ObservableCollection<IFulltext> nodes = new ObservableCollection<IFulltext>();
        public ObservableCollection<IFulltext> Nodes
        {
            get { return nodes; }
            set { SetProperty(ref nodes, value); }
        }

        public bool IsChecked { get; set; }

        public bool IsIncluded { get; set; }
        public bool IsExcluded { get; set; }

    }
}
