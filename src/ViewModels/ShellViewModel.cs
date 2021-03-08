using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using MahApps.Metro.Controls;
using NHotkey.Wpf;
using Stylet;
using tabbR.Application;
using tabbR.Application.Browser.Core;
using tabbR.Application.Browser.Models;
using tabbR.Application.Extensions;
using static tabbR.Bootstrapper;
using Mouse = FlaUI.Core.Input.Mouse;
using Point = System.Drawing.Point;
using Screen = Stylet.Screen;

namespace tabbR.ViewModels
{
    public class ShellViewModel : Conductor<Screen>
    {
        private readonly IBrowserTabFinder _browserTabFinder;
        private BindableCollection<SearchResultItemModel> _results;
        private readonly IWindowManager _windowManager;
        
        public BindableCollection<SearchResultItemModel> Results
        {
            get => _results;
            set => SetAndNotify(ref _results, value);
        }

        public SearchResultItemModel SelectedSearchResultItemModel { get; set; }

        private bool _isVisible;

        public bool IsVisible
        {
            get => _isVisible;
            set => SetAndNotify(ref _isVisible, value);
        }

        private string _querySearch;

        public string QuerySearch
        {
            get => _querySearch;
            set => SetAndNotify(ref _querySearch, value);
        }
        public void ShowWindow() => IsVisible = !IsVisible;

        public ShellViewModel(IBrowserTabFinder browserTabFinder, IWindowManager windowManager)
        {
            _browserTabFinder = browserTabFinder;
            _windowManager = windowManager;
            Results = new BindableCollection<SearchResultItemModel>();
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();
            this.Bind(model => model.QuerySearch, QuerySearchChanged);
            
            // initialize settings on first launch
            var hotKey = Tracker.GetValue<HotKey>(nameof(SettingsViewModel.HotKey));
            if(hotKey == null)
                ShowSettings();

            AddHotKey(ShowWindow);
        }

        public async Task KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            await QueryResultClicked();
        }

        public void ShowSettings()
        {
            var view = new SettingsViewModel();
            base.ActivateItem(view);
            var result = _windowManager.ShowDialog(view);
            if (result.HasValue && result.Value)
                AddHotKey(ShowWindow);
        }

        public void Exit() => Environment.Exit(0);

        public async Task QueryResultClicked()
        {
            try
            {
                var instance = SelectedSearchResultItemModel.Instance;
                WinApi.ShowWindowAsync(instance.Properties.NativeWindowHandle, WinApi.SW_SHOWNORMAL);
                WinApi.SetForegroundWindow(instance.Properties.NativeWindowHandle);

                instance.WaitUntilEnabled();
                instance.WaitUntilClickable();

                var clickableCoords =
                    new Point(SelectedSearchResultItemModel.TabItem.BoundingRectangle.ImmediateInteriorNorth().X,
                        SelectedSearchResultItemModel.TabItem.BoundingRectangle.ImmediateInteriorNorth().Y + 15);

                var savePos = System.Windows.Forms.Cursor.Position;
                Mouse.Position = clickableCoords;
                await Task.Delay(TimeSpan.FromMilliseconds(200));
                Mouse.LeftClick(clickableCoords);

                if (Tracker.GetValue<bool>(nameof(SettingsViewModel.GoBackToOrginalMousePosition)))
                    Mouse.Position = savePos;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Unhandled error occured.", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void QuerySearchChanged(object sender, PropertyChangedExtendedEventArgs<string> e)
        {
            Results.Clear();

            if (string.IsNullOrEmpty(e.NewValue))
                return;

            var results = await _browserTabFinder.Search(e.NewValue);
            if (!results.Any())
                return;

            Results.AddRange(results);
        }

        private void AddHotKey(Action callback)
        {
            HotkeyManager.Current.AddOrReplace("ShowWindow", GetHotKey(), (_, __) =>
            {
                callback?.Invoke();
            });
        }

        private KeyGesture GetHotKey()
        {
            var hotKey = Tracker.GetValue<HotKey>(nameof(SettingsViewModel.HotKey));
            var modifier = new KeyGesture(hotKey.Key, hotKey.ModifierKeys);
            return modifier;
        }
    }
}
