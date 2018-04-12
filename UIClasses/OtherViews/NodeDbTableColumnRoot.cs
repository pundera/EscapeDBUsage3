﻿using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.UIClasses.OtherViews
{
    public class NodeDbTableColumnRoot: NodeBase
    {
        public NodeDbTableColumnRoot(IEventAggregator evAgg) : base(evAgg)
        {

        }

        private ObservableCollection<NodeDbTableColumnsToExcel> nodes;
        public ObservableCollection<NodeDbTableColumnsToExcel> Nodes
        {
            get { return nodes; }
            set { SetProperty(ref nodes, value); }
        }
    }
}
