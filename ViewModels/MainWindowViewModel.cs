using EscapeDBUsage.Classes;
using EscapeDBUsage.Confirmations;
using EscapeDBUsage.Events;
using EscapeDBUsage.Helpers;
using EscapeDBUsage.InteractionRequests;
using EscapeDBUsage.UIClasses;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EscapeDBUsage.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IEventAggregator eventAgg;
        private ConnectConfirmation connectConfirmation;

        private DbConnection dbConnection;

        private bool isConnected = false;
        public bool IsConnected
        {
            get { return isConnected; }
            set { SetProperty(ref isConnected, value); }
        }

        private DatabaseSchemaViewModel databaseSchemaViewModel;
        public DatabaseSchemaViewModel DatabaseSchemaViewModel
        {
            get
            {
                return databaseSchemaViewModel;
            }
            set
            {
                SetProperty(ref databaseSchemaViewModel, value);
            }
        }

        public MainWindowViewModel(MainViewModel mainViewModel, DatabaseSchemaViewModel databaseSchemaViewModel, IEventAggregator evAgg, ConnectConfirmation connectConfirmation)
        {
            eventAgg = evAgg;

            eventAgg.GetEvent<ConnectEvent>().Subscribe((x) => {
                IsConnected = x.IsConnected;
                if (IsConnected) dbConnection = x;
            }); 

            this.connectConfirmation = connectConfirmation;
            MainViewModel = mainViewModel;
            DatabaseSchemaViewModel = databaseSchemaViewModel;

            ConnectRequest = new ConnectRequest(evAgg);
            Connect = new DelegateCommand(() => {
                DoConnect();
            });

            LoadDbSchema = new DelegateCommand(() =>
            {
                DoLoadDbSchema();
            }); 
        }

        private void DoLoadDbSchema()
        {
            MainViewModel.SelectedSprint.DbSchemaTables = DbSchemaHelper.ConvertDbSchemaTables(DbSchemaHelper.GetTablesWithColumns(dbConnection));
        }

        private string _title = "Escape DB Usage (Bindings between Excels, Tabs, Values and DB Tables)";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private void DoConnect()
        {
            //this.DatabaseConnection = new DbConnection() { IsConnected = false };
            connectConfirmation.Title = "Connect to Database";
            ConnectRequest.Notification = connectConfirmation;
            //connectConfirmation.ShowSignal = true;

            ConnectRequest.Raise(connectConfirmation, (result) =>
            {
                if (result != null && result.DatabaseConnection != null && result.DatabaseConnection.IsConnected)
                {
                    eventAgg.GetEvent<ConnectEvent>().Publish(result.DatabaseConnection);
                }
            });            
        }

        private MainViewModel mainViewModel;
        public MainViewModel MainViewModel
        {
            get
            {
                return mainViewModel;
            }
            set
            {
                SetProperty(ref mainViewModel, value);
            }
        }

        public ConnectRequest ConnectRequest { get; private set; }

        public ICommand Connect { get; private set; }
        public ICommand LoadDbSchema { get; private set; }
    }
}
