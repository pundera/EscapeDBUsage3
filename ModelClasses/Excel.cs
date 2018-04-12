using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.ModelClasses
{
    public class Excel: Base
    {
        [JsonProperty(Order = 3)]
        public List<Tab> Nodes { get; set; }
    }
}
