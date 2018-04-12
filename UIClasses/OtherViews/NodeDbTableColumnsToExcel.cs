using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.UIClasses.OtherViews
{
    public class NodeDbTableColumnsToExcel: NodeBase
    {
        public NodeDbTableColumnsToExcel(IEventAggregator evAgg) : base(evAgg)
        {

        }

        private ObservableCollection<NodeDbTableColumnsToExcelToTab> nodes;
        public ObservableCollection<NodeDbTableColumnsToExcelToTab> Nodes
        {
            get { return nodes; }
            set { SetProperty(ref nodes, value); }
        }
    }
}
