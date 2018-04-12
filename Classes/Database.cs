using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.Classes
{
    public class Database
    {
        public string Name { get; set; }
        public short Id { get; set; }
        public DateTime Created { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, Id: {1}, Created: {2}", Name, Id, Created);
        }
    }
}
