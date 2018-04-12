using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.ModelClasses.DbSchema
{
    public class SchemaDb : SchemaBaseClass
    {
        [JsonProperty(Order = 3)]    
        public Guid SprintGuid { get; set; }
        [JsonProperty(Order = 4)]
        public IList<SchemaDbTable> Tables { get; set; }
    }
}
