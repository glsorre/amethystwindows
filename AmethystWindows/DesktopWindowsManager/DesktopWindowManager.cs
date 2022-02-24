using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Vanara.PInvoke;
using WindowsDesktop;

using AmethystWindows.Settings;
using DebounceThrottle;
using System.Diagnostics;
using System.Windows.Interop;
using System.Threading.Tasks;

namespace AmethystWindows.DesktopWindowsManager
{
    partial class DesktopWindowsManager
    {
        public Dictionary<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>> Windows { get; }
        public Dictionary<Pair<VirtualDesktop, HMONITOR>, bool> WindowsSubscribed = new Dictionary<Pair<VirtualDesktop, HMONITOR>, bool>();

        public MainWindowViewModel mainWindowViewModel = App.Current.MainWindow.DataContext as MainWindowViewModel;

        private DebounceDispatcher debounceDispatcher = new DebounceDispatcher(100);
        private bool firstActivation = true;

        private readonly string[] FixedFilters = new string[] {
            "AmethystWindows",
            "AmethystWindowsPackaging",
            "Cortana",
            "Microsoft Spy++",
            "Task Manager",
        };

        private readonly string[] ModelViewPropertiesDraw = new string[] {
            "Padding",
            "LayoutPadding",
            "MarginTop",
            "MarginRight",
            "MarginBottom",
            "MarginLeft",
            "ConfigurableFilters",
            "DesktopMonitors",
            "Windows",
        };

        private readonly string[] ModelViewPropertiesDrawMonitor = new string[] {
            "DesktopMonitors",
            "Windows",
        };

        private readonly string[] ModelViewPropertiesSaveSettings = new string[] {
            "Padding",
            "LayoutPadding",
            "MarginTop",
            "MarginRight",
            "MarginBottom",
            "MarginLeft",
            "DesktopMonitors",
            "Filters",
            "Hotkeys",
        };

        public DesktopWindowsManager()
        {
            Windows = new Dictionary<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>>();

            mainWindowViewModel.PropertyChanged += MainWindowViewModel_PropertyChanged;
        }

        private void MainWindowViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug.WriteLine($"ModelViewChanged: {e.PropertyName}");
            if (e.PropertyName == "Hotkeys")
            {
                HWND mainWindowHandle = new WindowInteropHelper(App.Current.MainWindow).Handle;
                App.hooks.unsetKeyboardHook(mainWindowHandle);
                App.hooks.setKeyboardHook(mainWindowHandle, mainWindowViewModel.Hotkeys);
            }
            if (ModelViewPropertiesSaveSettings.Contains(e.PropertyName))
            {
                MySettings.Instance.LayoutPadding = mainWindowViewModel.LayoutPadding;
                MySettings.Instance.Padding = mainWindowViewModel.Padding;

                MySettings.Instance.MarginTop = mainWindowViewModel.MarginTop;
                MySettings.Instance.MarginRight = mainWindowViewModel.MarginRight;
                MySettings.Instance.MarginBottom = mainWindowViewModel.MarginBottom;
                MySettings.Instance.MarginLeft = mainWindowViewModel.MarginLeft;

                MySettings.Instance.Filters = mainWindowViewModel.ConfigurableFilters;
                MySettings.Instance.DesktopMonitors = mainWindowViewModel.DesktopMonitors.ToList();
                MySettings.Instance.Hotkeys = mainWindowViewModel.Hotkeys.ToList();
                
                MySettings.Save();
            }
            if (e.PropertyName == "ConfigurableFilters")
            {
                ClearWindows();
                CollectWindows();
            }
            if (ModelViewPropertiesDraw.Contains(e.PropertyName))
            {
                if (firstActivation)
                {
                    debounceDispatcher.Debounce(() => Draw());
                    firstActivation = false;
                }

                if (ModelViewPropertiesDrawMonitor.Contains(e.PropertyName)) debounceDispatcher.Debounce(() => Draw());
                else debounceDispatcher.Debounce(() => Draw(mainWindowViewModel.LastChangedDesktopMonitor));
            }
        }

        public void RotateMonitorClockwise(Pair<VirtualDesktop, HMONITOR> currentDesktopMonitor)
        {
            List<HMONITOR> virtualDesktopMonitors = Windows
                .Keys
                .Where(desktopMonitor => desktopMonitor.Key.Equals(currentDesktopMonitor.Key))
                .Select(desktopMonitor => desktopMonitor.Value)
                .ToList();

            HMONITOR nextMonitor = virtualDesktopMonitors.SkipWhile(x => x != currentDesktopMonitor.Value).Skip(1).DefaultIfEmpty(virtualDesktopMonitors[0]).FirstOrDefault();
            Pair<VirtualDesktop, HMONITOR> nextDesktopMonitor = new Pair<VirtualDesktop, HMONITOR>(currentDesktopMonitor.Key, nextMonitor);

            User32.SetForegroundWindow(Windows[nextDesktopMonitor].FirstOrDefault().Window);
        }

        public void RotateMonitorCounterClockwise(Pair<VirtualDesktop, HMONITOR> currentDesktopMonitor)
        {
            List<HMONITOR> virtualDesktopMonitors = Windows
                .Keys
                .Where(desktopMonitor => desktopMonitor.Key.Equals(currentDesktopMonitor.Key))
                .Select(desktopMonitor => desktopMonitor.Value)
                .ToList();

            HMONITOR nextMonitor = virtualDesktopMonitors.TakeWhile(x => x != currentDesktopMonitor.Value).Skip(1).DefaultIfEmpty(virtualDesktopMonitors[0]).FirstOrDefault();
            Pair<VirtualDesktop, HMONITOR> nextDesktopMonitor = new Pair<VirtualDesktop, HMONITOR>(currentDesktopMonitor.Key, nextMonitor);

            User32.SetForegroundWindow(Windows[nextDesktopMonitor].FirstOrDefault().Window);
        }

        private void SubscribeWindowsCollectionChanged(Pair<VirtualDesktop, HMONITOR> desktopMonitor, bool enabled)
        {
            if (!enabled) Windows[desktopMonitor].CollectionChanged -= Windows_CollectionChanged;
            else if (!WindowsSubscribed[desktopMonitor]) Windows[desktopMonitor].CollectionChanged += Windows_CollectionChanged;

            WindowsSubscribed[desktopMonitor] = enabled;
        }

    }
}
