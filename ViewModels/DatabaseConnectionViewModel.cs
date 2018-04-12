using EscapeDBUsage.ModelClasses.DbSchema;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.ViewModels
{
    public class DatabaseConnectionViewModel: BindableBase
    {
        public DatabaseConnectionViewModel()
        {
            DatabaseConnectionRequest = new InteractionRequest<DatabaseConnection>();
        }

        public InteractionRequest<DatabaseConnection> DatabaseConnectionRequest { get; private set; }

        private void RaiseConfirmation()
        {
            this.DatabaseConnectionRequest.Raise(
                new Confirmation { Content = "Confirmation Message", Title = "Confirmation" },
                c => { InteractionResultMessage = c.Confirmed ? "The user accepted." : "The user cancelled."; });
        }
    }
}
