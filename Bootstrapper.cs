using EscapeDBUsage.Views;
using System.Windows;
using Prism.Modularity;
using Autofac;
using Prism.Autofac;
using Prism.Regions;
using Prism.Mvvm;
using EscapeDBUsage.ViewModels;
using EscapeDBUsage.Confirmations;
using Prism.Events;
using EscapeDBUsage.UIClasses;

namespace EscapeDBUsage
{
    class Bootstrapper : AutofacBootstrapper
    {
        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            base.ConfigureContainerBuilder(builder);

            var eventAggregator = new EventAggregator();
            builder.RegisterInstance<IEventAggregator>(eventAggregator).As<IEventAggregator>().SingleInstance();
            
            // just one instance in all application - combos etc..
            builder.RegisterType<DatabaseSchemaViewModel>().As<DatabaseSchemaViewModel>().SingleInstance(); 
            builder.RegisterType<MainViewModel>().As<MainViewModel>().SingleInstance();

            // one instance --> use in multiple viewmodels
            builder.RegisterInstance<UISprints>(new UISprints()).As<UISprints>().SingleInstance();
            
            // just one instance as well 
            // well, works as hell .. :-)  
            var confirmation = new ConnectConfirmation()
            {
                DatabaseConnection = new Classes.DbConnection()
                {
                    IsConnected = false
                },
                Title = "Connect to Database",
                Request = new InteractionRequests.ConnectRequest(eventAggregator) 
            };
            confirmation.Request.Notification = confirmation;
            builder.RegisterInstance<ConnectConfirmation>(confirmation).As<ConnectConfirmation>().SingleInstance();
        }

        protected override DependencyObject CreateShell()
        {            
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();

            // View discovery
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion", () => Container.Resolve<MainView>());
            regionManager.RegisterViewWithRegion("Sprints", () => Container.Resolve<SprintsView>());
            regionManager.RegisterViewWithRegion("DatabaseSchemaView", () => Container.Resolve<DatabaseSchemaView>());
        }

        protected override void ConfigureModuleCatalog()
        {
            var moduleCatalog = (ModuleCatalog)ModuleCatalog;
            //moduleCatalog.AddModule(typeof(YOUR_MODULE));
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();
        }

    }
}
