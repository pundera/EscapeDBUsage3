using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.UIClasses.DatabaseSchema
{
    public class UIDatabaseConnection: BindableBase
    {
        public string Server { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public string Database { get; set; }
    }
}
