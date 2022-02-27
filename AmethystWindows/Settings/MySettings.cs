using AmethystWindows.DesktopWindowsManager;
using AmethystWindows.Hotkeys;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Input;
using Vanara.PInvoke;
using WindowsDesktop;

namespace AmethystWindows.Settings
{
    public class MySettings : SettingsManager<MySettings>
    {
        public int Padding { get; set; } = 0;
        public int Step { get; set; } = 25;
        public int LayoutPadding { get; set; } = 4;
        public int MarginTop { get; set; } = 0;
        public int MarginRight { get; set; } = 0;
        public int MarginBottom { get; set; } = 0;
        public int MarginLeft { get; set; } = 0;
        public int VirtualDesktops { get; set; } = 0;

        public bool Disabled { get; set; } = false;

        public List<Pair<string, string>> Filters { get; set; } = new List<Pair<string, string>>();
        public List<Pair<string, string>> Additions { get; set; } = new List<Pair<string, string>>();
        [JsonConverter(typeof(DesktopMonitorsConverter))]
        public List<ViewModelDesktopMonitor> DesktopMonitors = new List<ViewModelDesktopMonitor>();
        public List<ViewModelHotkey> Hotkeys { get; set; } = new List<ViewModelHotkey>();
    }
}
