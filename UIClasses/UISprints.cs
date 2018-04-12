using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.UIClasses
{
    public class UISprints: BindableBase
    {
        public UISprints()
        {
            Sprints = new ObservableCollection<UISprint>();
        }

        public ObservableCollection<UISprint> Sprints
        {
            get;
            private set;

        }
    }
}
