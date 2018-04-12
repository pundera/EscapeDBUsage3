using EscapeDBUsage.UIClasses;
using EscapeDBUsage.ViewModels;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.Notifications
{
    public class AddTablesAndColumnsNotification: Confirmation
    {
        public AddTablesAndColumnsNotification()
        {

        }

        public UISprint SelectedSprint { get; set; }

    }
}
