using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Jot;
using Stylet;
using StyletIoC;
using tabbR.Application.Browser;
using tabbR.Application.Browser.Core;
using tabbR.Modules;
using tabbR.ViewModels;

namespace tabbR
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        public static Tracker Tracker = new Tracker();
        private static readonly Mutex _mutex = new Mutex(false, "github.com/atresnjo-tabbR");

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            base.ConfigureIoC(builder);
            builder.Bind<ITabFinder>().To<FirefoxTabFinder>().WithKey("Firefox");
            builder.Bind<ITabFinder>().To<ChromeTabFinder>().WithKey("Chrome");
            builder.Bind<IBrowserTabFinder>().To<BrowserTabFinder>().InSingletonScope();
            builder.Bind<ITabFactory>().ToAbstractFactory();
            builder.AddModule(new SettingsModule());
        }

        protected override void OnStart()
        {
            if (!_mutex.WaitOne(TimeSpan.FromSeconds(5), false))
            {
                Environment.Exit(0);
                return;
            }
            base.OnStart();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _mutex.ReleaseMutex();
        }


        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs ex)
        {
            base.OnUnhandledException(ex);
            MessageBox.Show(ex.Exception.ToString(), "Unhandled error occured.", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
    