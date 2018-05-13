using EscapeDBUsage.Interfaces;
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
    public class NodeRoot: NodeBase
    {
        public NodeRoot(IEventAggregator evAgg, MainViewModel viewModel):base(evAgg)
        {

        }

        private MainViewModel viewModel; 

        private ObservableCollection<IFulltext> nodes = new ObservableCollection<IFulltext>();
        public ObservableCollection<IFulltext> Nodes
        {
            get { return nodes; }
            set { SetProperty(ref nodes, value); }
        }

        public NodeRoot Copy()
        {
            var newRoot = new NodeRoot(EventAggregator, viewModel)
            {
                Nodes = new ObservableCollection<IFulltext>()
            };


            //newRoot.Nodes = new ObservableCollection<IFulltext>(Nodes.Select(n => new NodeExcel(EventAggregator, newRoot, viewModel)
            //{
            //    Name = n.Name,
            //    Description = (n as NodeBase).Description,
            //    IsExpanded = true,
            //    Nodes = new ObservableCollection<NodeTab>(n.Nodes.ToList().Select(t => new NodeTab(EventAggregator, (NodeBase)n, viewModel)
            //    {
            //        Name = t.Name,
            //        Description = (t as NodeBase).Description,
            //        Nodes = new ObservableCollection<NodeDbTable>(t.Nodes.ToList().Select(table => new NodeDbTable(EventAggregator, t as NodeBase, viewModel)
            //        {
            //            Name = table.Name,
            //            Description = (table as NodeBase).Description,
            //            Nodes = new ObservableCollection<NodeDbColumn>(table.Nodes.ToList().Select(c => new NodeDbColumn(EventAggregator, table, viewModel)
            //            {
            //                Name = c.Name,
            //                Description = (c as NodeBase).Description
            //            }))
            //        }))
            //    }))
            //}));

            return newRoot;
        }
    }
}
