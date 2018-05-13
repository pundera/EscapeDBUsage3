using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.Interfaces
{
    public interface IFulltext
    {
        string Name { get; set; }
        bool IsVisible { get; set; }
        bool IsChecked { get; set; }
        bool IsExpanded { get; set; }

        ObservableCollection<IFulltext> Nodes { get; set; }
    }
}
