using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.UIClasses.OtherViews
{
    public class NodeDbTableToExcel: NodeBase
    {
        public NodeDbTableToExcel(IEventAggregator evAgg):base(evAgg)
        {

        }

        private ObservableCollection<NodeDbTableToExcelToTab> nodes;
        public ObservableCollection<NodeDbTableToExcelToTab> Nodes
        {
            get { return nodes; }
            set { SetProperty(ref nodes, value); }
        }
    }
}
