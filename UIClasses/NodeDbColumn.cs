using EscapeDBUsage.Interfaces;
using EscapeDBUsage.UIClasses.DatabaseSchema;
using EscapeDBUsage.ViewModels;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.UIClasses
{
    public class NodeDbColumn: NodeBase, IFulltext
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

        public ObservableCollection<IFulltext> Nodes { get { return null; } set { throw new NotImplementedException(); } }

        public bool IsChecked { get; set; }

        public bool IsIncluded { get; set; }
        public bool IsExcluded { get; set; }

        public NodeDbSchemaColumn ColumnInfo { get; set; }

    }
}
