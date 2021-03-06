using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Portierniaktosiedzi.Database;
using Portierniaktosiedzi.ViewModels;
using Xceed.Wpf.Toolkit;

namespace Portierniaktosiedzi
{
    public class AppBootstrapper : BootstrapperBase
    {
        SimpleContainer container;

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            container = new SimpleContainer();
            container.Singleton<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();
            container.Singleton<IConnectedRepository, ConnectedRepository>();
            container.PerRequest<IShell, ShellViewModel>();

            ConventionManager.AddElementConvention<DateTimeUpDown>(DateTimeUpDown.ValueProperty, "Value", "ValueChangedEvent"); //to ma sens xD?
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            DisplayRootViewFor<IShell>();
        }
    }
}