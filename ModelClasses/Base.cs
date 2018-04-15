using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.ModelClasses
{
    public class Base
    {
        [JsonProperty(Order = 0)]
        public Guid Guid { get; set; }

        [JsonProperty(Order = 1)]
        public string Name
        {
            get;
            set;
        }
        [JsonProperty(Order = 2)]
        public string Description
        {
            get;
            set;
        }

    }
}
