using EscapeDBUsage.Notifications;
using EscapeDBUsage.UIClasses;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EscapeDBUsage.ViewModels
{
    public class AddTablesAndColumnsViewModel : BindableBase, IInteractionRequestAware
    {
        /// <summary>
        /// just for design..
        /// </summary>
        public AddTablesAndColumnsViewModel()
        {
            Ok = new DelegateCommand(() => DoOk());
            Cancel = new DelegateCommand(() => DoCancel());
        }

        public AddTablesAndColumnsViewModel(DatabaseSchemaViewModel vm)
        {

            Ok = new DelegateCommand(() => DoOk());
            Cancel = new DelegateCommand(() => DoCancel());

            ViewModel = vm;
        }

        private void DoCancel()
        {
            if (notification != null)
                ((AddTablesAndColumnsNotification)notification).Confirmed = false;
            FinishInteraction();
        }

        private void DoOk()
        {
            if (notification != null)
                ((AddTablesAndColumnsNotification)notification).Confirmed = true;
            FinishInteraction();
        }

        public ICommand Ok { get; private set; }
        public ICommand Cancel { get; private set; }

        public Action FinishInteraction
        {
            get; set;
        }

        private INotification notification;
        public INotification Notification
        {
            get
            {
                return notification;
            }
            set
            {
                SetProperty(ref notification, value);
            }
        }


        public DatabaseSchemaViewModel ViewModel { get; set; }

    }
}
