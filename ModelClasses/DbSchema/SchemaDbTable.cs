using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.ModelClasses.DbSchema
{
    public class SchemaDbTable: SchemaBaseClass
    {
        [JsonProperty(Order = 3)]
        public List<SchemaDbColumn> Columns { get; set; }
    }
}
