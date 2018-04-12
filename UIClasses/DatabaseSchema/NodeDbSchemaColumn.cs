using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.UIClasses.DatabaseSchema
{
    public class NodeDbSchemaColumn: NodeDbSchemaBase
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
    }
}
