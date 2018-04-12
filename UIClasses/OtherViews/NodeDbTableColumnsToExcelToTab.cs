using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.UIClasses.OtherViews
{
    public class NodeDbTableColumnsToExcelToTab:NodeBase
    {
        public NodeDbTableColumnsToExcelToTab(IEventAggregator eventAggregator) : base(eventAggregator)
        {

        }

        private ObservableCollection<NodeTab> nodes;
        public ObservableCollection<NodeTab> Nodes
        {
            get { return nodes; }
            set { SetProperty(ref nodes, value); }
        }
    }
}
