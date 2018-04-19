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

            var uiSprints = new UISprints();
            var databaseViewModel = new DatabaseSchemaViewModel(eventAggregator);
            var mainViewModel = new MainViewModel(eventAggregator, uiSprints, databaseViewModel);
            var pathViewModel = new PathViewModel(eventAggregator, mainViewModel);

            // just one instance in all application - combos etc..
            builder.RegisterInstance<UISprints>(uiSprints).As<UISprints>().SingleInstance();
            builder.RegisterInstance<DatabaseSchemaViewModel>(databaseViewModel).As<DatabaseSchemaViewModel>().SingleInstance();
            builder.RegisterInstance<MainViewModel>(mainViewModel).As<MainViewModel>().SingleInstance();
            builder.RegisterInstance<PathViewModel>(pathViewModel).As<PathViewModel>().SingleInstance();
            
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
