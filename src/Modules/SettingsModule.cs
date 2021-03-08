using System.Windows.Input;
using MahApps.Metro.Controls;
using StyletIoC;
using tabbR.ViewModels;
using static tabbR.Bootstrapper;

namespace tabbR.Modules
{
    public class SettingsModule : StyletIoCModule
    {
        protected override void Load()
        {
            Tracker.Configure<SettingsViewModel>().Property(x => x.IsChromeEnabled, true);
            Tracker.Configure<SettingsViewModel>().Property(x => x.IsFirefoxEnabled, true);
            Tracker.Configure<SettingsViewModel>().Property(x => x.GoBackToOrginalMousePosition, false);
            Tracker.Configure<SettingsViewModel>().Property(x => x.HotKey, new HotKey(Key.Space, ModifierKeys.Alt));
        }
    }
}
