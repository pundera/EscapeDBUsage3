using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.UIClasses.DatabaseSchema
{
    public class NodeDbSchemaTable: NodeDbSchemaBase
    {
        private ObservableCollection<NodeDbSchemaColumn> columns;
        public ObservableCollection<NodeDbSchemaColumn> Columns { get { return columns; } set { SetProperty(ref columns, value); } }
    }
}
