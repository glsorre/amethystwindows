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
        private static ModifierKeys Modifier1 = ModifierKeys.Shift | ModifierKeys.Alt;
        private static ModifierKeys Modifier2 = ModifierKeys.Shift | ModifierKeys.Alt | ModifierKeys.Windows;
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
        public List<ViewModelHotkey> Hotkeys { get; set; } = new List<ViewModelHotkey>()
        {
            new ViewModelHotkey() { Command = "rotateLayoutClockwise", Hotkey = new Hotkey(Key.Space, Modifier1) },
            new ViewModelHotkey() { Command = "rotateLayoutCounterclockwise", Hotkey = new Hotkey(Key.Space, Modifier2) },

            new ViewModelHotkey() { Command = "setMainPane", Hotkey = new Hotkey(Key.Enter, Modifier1) },

            new ViewModelHotkey() { Command = "swapFocusedCounterclockwise", Hotkey = new Hotkey(Key.H, Modifier1) },
            new ViewModelHotkey() { Command = "swapFocusedClockwise", Hotkey = new Hotkey(Key.L, Modifier1) },

            new ViewModelHotkey() { Command = "swapFocusCounterclockwise", Hotkey = new Hotkey(Key.J, Modifier1) },
            new ViewModelHotkey() { Command = "swapFocusClockwise", Hotkey = new Hotkey(Key.K, Modifier1) },

            new ViewModelHotkey() { Command = "moveFocusPreviousScreen", Hotkey = new Hotkey(Key.P, Modifier1) },
            new ViewModelHotkey() { Command = "moveFocusNextScreen", Hotkey = new Hotkey(Key.N, Modifier1) },

            new ViewModelHotkey() { Command = "expandMainPane", Hotkey = new Hotkey(Key.L, Modifier2) },
            new ViewModelHotkey() { Command = "shrinkMainPane", Hotkey = new Hotkey(Key.H, Modifier2) },

            new ViewModelHotkey() { Command = "moveFocusedPreviousScreen", Hotkey = new Hotkey(Key.K, Modifier1) },
            new ViewModelHotkey() { Command = "moveFocusedNextScreen", Hotkey = new Hotkey(Key.J, Modifier1) },

            new ViewModelHotkey() { Command = "redraw", Hotkey = new Hotkey(Key.Z, Modifier1) },

            new ViewModelHotkey() { Command = "moveFocusedNextSpace", Hotkey = new Hotkey(Key.Left, Modifier2) },
            new ViewModelHotkey() { Command = "moveFocusedPreviousSpace", Hotkey = new Hotkey(Key.Right, Modifier2) },

            new ViewModelHotkey() { Command = "moveFocusedToSpace1", Hotkey = new Hotkey(Key.D1, Modifier2) },
            new ViewModelHotkey() { Command = "moveFocusedToSpace2", Hotkey = new Hotkey(Key.D2, Modifier2) },
            new ViewModelHotkey() { Command = "moveFocusedToSpace3", Hotkey = new Hotkey(Key.D3, Modifier2) },
            new ViewModelHotkey() { Command = "moveFocusedToSpace4", Hotkey = new Hotkey(Key.D4, Modifier2) },
            new ViewModelHotkey() { Command = "moveFocusedToSpace5", Hotkey = new Hotkey(Key.D5, Modifier2) },
        };
    }
}
