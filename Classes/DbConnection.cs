using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.Classes
{
    public class DbConnection
    {
        public bool IsConnected { get; set; }

        public string ServerName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }

        public override string ToString()
        {
            return string.Format("Server name: {0}, Login: {1}, Database: {2}, Connected: {3}", ServerName, Login, Database, IsConnected);
        }
    }
}
