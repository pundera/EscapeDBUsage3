using EscapeDBUsage.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.UIClasses.DatabaseSchema
{
    public class NodeDbSchemaTable: NodeDbSchemaBase, IFulltext
    {
        private ObservableCollection<IFulltext> nodes;
        public ObservableCollection<IFulltext> Nodes { get { return nodes; } set { SetProperty(ref nodes, value); } }
    }
}
