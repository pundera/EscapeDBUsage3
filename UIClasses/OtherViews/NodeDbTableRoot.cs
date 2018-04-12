using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.UIClasses.OtherViews
{
    public class NodeDbTableRoot : NodeBase
    {
        public NodeDbTableRoot(IEventAggregator evAgg) : base(evAgg)
        {

        }

        private ObservableCollection<NodeDbTableToExcel> nodes;
        public ObservableCollection<NodeDbTableToExcel> Nodes
        {
            get { return nodes; }
            set { SetProperty(ref nodes, value); }
        }
    }
}
