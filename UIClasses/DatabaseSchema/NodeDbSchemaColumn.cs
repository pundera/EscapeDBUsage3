using EscapeDBUsage.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.UIClasses.DatabaseSchema
{
    public class NodeDbSchemaColumn: NodeDbSchemaBase, IFulltext
    {
        public string DataType { get; set; }
        public bool IsComputed { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsNullable { get; set; }
        public string ObjectType { get; set; }
        public int Position { get; set; }
        public string TableName { get; set; }
        public int? Length { get; set; }

        public object DefaultValue { get; set; }
        public ObservableCollection<IFulltext> Nodes { get; set; }

        public bool IsIncluded { get; set; }
        public bool IsExcluded { get; set; }
    }
}
