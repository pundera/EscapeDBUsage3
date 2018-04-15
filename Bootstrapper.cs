using EscapeDBUsage.Views;
using System.Windows;
using Prism.Modularity;
using Autofac;
using Prism.Autofac;
using Prism.Regions;
using Prism.Mvvm;
using EscapeDBUsage.ViewModels;
using Prism.Events;
using EscapeDBUsage.UIClasses;
using EscapeDBUsage.Notifications;

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
            builder.RegisterType<PathViewModel>().As<PathViewModel>().SingleInstance();
            builder.RegisterType<DatabaseSchemaViewModel>().As<DatabaseSchemaViewModel>().SingleInstance(); 
            builder.RegisterType<MainViewModel>().As<MainViewModel>().SingleInstance();

            // one instance --> use in multiple viewmodels
            builder.RegisterInstance<UISprints>(new UISprints()).As<UISprints>().SingleInstance();
            
            // just one instance as well 
            // well... works as hell! :-) 
            // .. bell?? no.. try to study..
            // bez ostudy.. :-)
            var notification = new ConnectNotification()
            {
                DbConnection = new Classes.DbConnection()
                {
                    IsConnected = false
                },
                Title = "Connect to Database"
            };
            builder.RegisterInstance(notification).As<ConnectNotification>().SingleInstance();
        }

        protected override DependencyObject CreateShell()
        {            
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            // View discovery
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion", () => Container.Resolve<MainView>());
            regionManager.RegisterViewWithRegion("PathRegion", () => Container.Resolve<PathView>());
            regionManager.RegisterViewWithRegion("Sprints", () => Container.Resolve<SprintsView>());
            regionManager.RegisterViewWithRegion("DatabaseSchemaView", () => Container.Resolve<DatabaseSchemaView>());

            Application.Current.MainWindow.Show();
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
