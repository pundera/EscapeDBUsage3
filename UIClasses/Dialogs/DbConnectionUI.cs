using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.UIClasses.Dialogs
{
    public class DbConnectionUI: BindableBase
    {
        private bool isConnected;
        public bool IsConnected { get { return isConnected; } set { SetProperty(ref isConnected, value); } }

        private string serverName;
        public string ServerName { get { return serverName; } set { SetProperty(ref serverName, value); } }
        private string login;
        public string Login { get { return login; } set { SetProperty(ref login, value); } }
        private string password;
        public string Password { get { return password; } set { SetProperty(ref password, value); } }
        private string database;
        public string Database { get { return database; } set { SetProperty(ref database, value); } }

        public override string ToString()
        {
            return string.Format("Server name: {0}, Login: {1}, Database: {2}, Connected: {3}", ServerName, Login, Database, IsConnected);
        }

    }
}
