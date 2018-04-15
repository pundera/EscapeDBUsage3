using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.UIClasses.Path
{
    public class PathItem: BindableBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private Guid guid;
        public Guid Guid
        {
            get { return guid; }
            set { SetProperty(ref guid, value); }
        }
    }
}
