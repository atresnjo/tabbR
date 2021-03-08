using MahApps.Metro.Controls;
using Stylet;

namespace tabbR.ViewModels
{
    public class SettingsViewModel : Screen
    {
        public bool IsFirefoxEnabled { get; set; }
        public bool IsChromeEnabled { get; set; }
        public bool GoBackToOrginalMousePosition { get; set; }

        private HotKey _hotKey;
        public HotKey HotKey
        {
            get => _hotKey;
            set => SetAndNotify(ref _hotKey, value);
        }

        public SettingsViewModel() => Bootstrapper.Tracker.Track(this);

        public void Close() => RequestClose(false);

        protected override void OnClose()
        {
            base.OnClose();
            Bootstrapper.Tracker.StopTracking(this);
        }

        public void Save()
        {
            Bootstrapper.Tracker.Persist(this);
            RequestClose(true);
        }
    }
}
