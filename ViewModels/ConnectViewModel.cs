using EscapeDBUsage.Classes;
using EscapeDBUsage.Confirmations;
using EscapeDBUsage.Helpers;
using EscapeDBUsage.UIClasses.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace EscapeDBUsage.ViewModels
{
    public class ConnectViewModel: BindableBase
    {
        private ConnectConfirmation connectConfirmation;
        public ConnectViewModel(ConnectConfirmation connectConfirmation)
        {
            this.connectConfirmation = connectConfirmation;

            var db = this.connectConfirmation.DatabaseConnection;

            DbConnection = new DbConnectionUI()
            {
                IsConnected = false,
                ServerName = db.ServerName ?? @"localhost\SQLEXPRESS",
                Login = db.Login ?? "pundera"
            };

            Connect = new DelegateCommand(() => DoConnect());
            Ok = new DelegateCommand(() => DoOk());
            Cancel = new DelegateCommand(() => DoCancel());
            PasswordChanged = new DelegateCommand<PasswordBox>((p) => DoPasswordChange(p));
        }

        private void DoCancel()
        {
            connectConfirmation.Request.FinishInteraction();
            connectConfirmation.CloseSignal = true;
        }

        private void DoOk()
        {
            if (DbConnection.IsConnected)
            {
                // always should be... :-) (this.IsConnected => true..)
                connectConfirmation.DatabaseConnection = new Classes.DbConnection() { 
                    IsConnected = true, 
                    ServerName = DbConnection.ServerName, 
                    Login =  DbConnection.Login, 
                    Password = DbConnection.Password, 
                    Database = DbConnection.Database = SelectedDatabase.Name
                };
                connectConfirmation.Request.FinishInteraction();
                connectConfirmation.CloseSignal = true;
            }
        }

        private void DoPasswordChange(PasswordBox p)
        {
            var pass = p.Password;

            password = pass;
        }

        private string password;

        private async void DoConnect()
        {
            IsConnecting = true;
            IsConnected = false;
            DbConnection.IsConnected = false;

            Message = "Connecting...";
            Error = false;

            IsEnabled = false;
            DbConnection.Password = password;
            Databases = null; 

            var isNotFillIn = string.IsNullOrEmpty(DbConnection.Password) || string.IsNullOrEmpty(DbConnection.Login) || string.IsNullOrEmpty(DbConnection.ServerName);

            if (isNotFillIn)
            {
                Message = "Please fill in all properties...(Server, User name and Password...) :-)";
                Error = true;

                IsEnabled = true;
                IsConnecting = false;

                return;
            }

            var connString = ConnectionHelper.ToConnectionString(new DbConnection() { ServerName = DbConnection.ServerName, Login = DbConnection.Login, Password = DbConnection.Password });
           
            try
            {
                var connection = new SqlConnection(connString);
                await connection.OpenAsync().ContinueWith((result) => {
                    if (result.IsCompleted && !result.IsFaulted)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Message = "Success!!! Connected!!! :-)";
                            Error = false;

                            Databases = new ObservableCollection<Database>();
                            var databases = connection.GetSchema("Databases");
                            foreach (DataRow database in databases.Rows)
                            {
                                var databaseName = database.Field<String>("database_name");
                                var dbID = database.Field<short>("dbid");
                                var creationDate = database.Field<DateTime>("create_date");
                                var db = new Database() { Name = databaseName, Id = dbID, Created = creationDate };
                                Databases.Add(db);
                            }

                            if (Databases.Count > 0) SelectedDatabase = Databases.First();
                            DbConnection.IsConnected = true;
                            IsConnected = true;
                        });
                    }
                    if (result.IsFaulted)
                    {
                        var ex = result.Exception;
                        Message = ex.InnerException!=null ? ex.InnerException.Message : ex.Message;
                        Error = true;
                    }
                });
            }
            catch (Exception ex)
            {
                Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Error = true;
            }
            finally
            {
                IsEnabled = true;
                IsConnecting = false;
            }
        }

        private DbConnectionUI dbConnection;
        public DbConnectionUI DbConnection { get { return dbConnection; } set { SetProperty(ref dbConnection,value); } }

        public ICommand Connect { get; private set; }
        public ICommand Ok { get; private set; }
        public ICommand Cancel { get; private set; }
        public ICommand PasswordChanged { get; private set; }

        private string message;
        public string Message { get { return message; } set { SetProperty(ref message, value); } }

        private bool error = false;
        public bool Error { get { return error; } set { SetProperty(ref error, value); } }

        private ObservableCollection<Database> databases;
        public ObservableCollection<Database> Databases {
            get { return databases; }
            set { SetProperty(ref databases, value); }
        }

        private Database selectedDatabase;
        public Database SelectedDatabase { get { return selectedDatabase; } set { SetProperty(ref selectedDatabase, value); } }

        private bool isEnabled = true;
        public bool IsEnabled { get { return isEnabled; } set { SetProperty(ref isEnabled, value); } }

        private bool isConnecting = false;
        public bool IsConnecting { get { return isConnecting; } set { SetProperty(ref isConnecting, value); } }

        private bool isConnected = false;
        public bool IsConnected { get { return isConnected; } set { SetProperty(ref isConnected, value); } }
    }
}
