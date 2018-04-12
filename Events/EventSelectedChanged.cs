using EscapeDBUsage.UIClasses;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.Events
{
    public class EventSelectedChanged: PubSubEvent<NodeBase>
    {
        public NodeBase Selected { get; set; }
    }
}
