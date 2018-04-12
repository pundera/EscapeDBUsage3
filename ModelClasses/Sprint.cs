using EscapeDBUsage.ModelClasses.DbSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.ModelClasses
{
    public class Sprint
    {
        public Guid Guid { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public List<Excel> Excels { get; set; }

        public List<SchemaDbTable> DbSchemaTables { get; set; }
    }
}
