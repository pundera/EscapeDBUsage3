using EscapeDBUsage.Classes;
using EscapeDBUsage.InteractionRequests;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.Confirmations
{
    public class ConnectConfirmation: BindableBase, INotification
    {
        public DbConnection DatabaseConnection { get; set; }
        public ConnectRequest Request { get; set; }

        private bool showSignal = false;
        public bool ShowSignal { get { return showSignal; } set { SetProperty(ref showSignal, value); } }

        private bool closeSignal = false;
        public bool CloseSignal { get { return closeSignal; } set { SetProperty(ref closeSignal, value); } }

        public object Content
        {
            get; set;
        }

        public string Title
        {
            get;
            set;
        }
    }
}
