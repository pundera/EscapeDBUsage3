using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.ModelClasses.DbSchema
{
    internal class DatabaseConnection: INotification
    {
        public string Server { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public string Database { get; set; }

        private bool ok = false;
        public bool Ok { get { return ok; } set { ok = value; } }

        public object Content
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Title
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
