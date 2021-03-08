using Jot;
using tabbR.ViewModels;

namespace tabbR.Application.Extensions
{
    public static class TrackerExtensions
    {
        public static T GetValue<T>(this Tracker tracker, string key)
        {
            var data = tracker.Store.GetData(nameof(SettingsViewModel));
            if (data.ContainsKey(key))
                return data[key] is T t ? t : default;

            return default;
        }
    }
}
