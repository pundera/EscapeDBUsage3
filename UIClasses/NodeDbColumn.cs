using EscapeDBUsage.ViewModels;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.UIClasses
{
    public class NodeDbColumn: NodeBase
    {
        public NodeDbColumn(IEventAggregator eventAggregator, NodeDbTable dbTable, MainViewModel viewModel) : base(eventAggregator)
        {
            this.viewModel = viewModel;
            NodeDbTable = dbTable;
        }

        private MainViewModel viewModel;

        private NodeDbTable nodeDbTable;
        public NodeDbTable NodeDbTable
        {
            get { return nodeDbTable; }
            set { SetProperty(ref nodeDbTable, value); }
        }

    }
}
