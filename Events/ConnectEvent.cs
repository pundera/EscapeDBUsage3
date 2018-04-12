using EscapeDBUsage.Classes;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.Events
{
    public class ConnectEvent : PubSubEvent<DbConnection>
    {
    }
}
