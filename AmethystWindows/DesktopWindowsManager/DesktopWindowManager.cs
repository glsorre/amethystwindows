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
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AmethystWindowsTests")]
namespace AmethystWindows.DesktopWindowsManager
{
    partial class DesktopWindowsManager
    {
        public Dictionary<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>> Windows { get; }
        public Dictionary<Pair<VirtualDesktop, HMONITOR>, bool> WindowsSubscribed = new Dictionary<Pair<VirtualDesktop, HMONITOR>, bool>();

        public ObservableCollection<DesktopWindow> ExcludedWindows { get; }

        public MainWindowViewModel mainWindowViewModel = App.Current != null ? App.Current.MainWindow.DataContext as MainWindowViewModel : new MainWindowViewModel();

        private DebounceDispatcher debounceDispatcher = new DebounceDispatcher(100);

        private readonly string[] FixedFilters = new string[] {
            "AmethystWindows",
            "AmethystWindowsPackaging",
            "Cortana",
            "Microsoft Spy++",
            "Task Manager",
        };

        public readonly string[] FixedExcludedFilters = new string[] {
            "Settings",
        };

        private readonly string[] ModelViewPropertiesDraw = new string[] {
            "Padding",
            "LayoutPadding",
            "MarginTop",
            "MarginRight",
            "MarginBottom",
            "MarginLeft",
            "ConfigurableFilters",
            "ConfigurableAdditions",
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
            "Additions",
            "Filters",
            "Hotkeys",
        };

        public DesktopWindowsManager()
        {
            Windows = new Dictionary<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>>();
            ExcludedWindows = new ObservableCollection<DesktopWindow>();

            ExcludedWindows.CollectionChanged += ExcludedWindows_CollectionChanged;
            mainWindowViewModel.PropertyChanged += MainWindowViewModel_PropertyChanged;
        }

        private void ExcludedWindows_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!App.Current.MainWindow.Equals(null))
            {
                mainWindowViewModel.UpdateExcludedWindows();
            }
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
                MySettings.Instance.Additions = mainWindowViewModel.ConfigurableAdditions;
                MySettings.Instance.DesktopMonitors = mainWindowViewModel.DesktopMonitors.ToList();
                MySettings.Instance.Hotkeys = mainWindowViewModel.Hotkeys.ToList();
                
                MySettings.Save();
            }
            if (e.PropertyName == "ConfigurableFilters" || e.PropertyName == "ConfigurableAdditions")
            {
                ClearWindows();
                CollectWindows();
            }
            if (ModelViewPropertiesDraw.Contains(e.PropertyName))
            {
                if (ModelViewPropertiesDrawMonitor.Contains(e.PropertyName) && mainWindowViewModel.LastChangedDesktopMonitor.Key != null) debounceDispatcher.Debounce(() => Draw(mainWindowViewModel.LastChangedDesktopMonitor));
                else debounceDispatcher.Debounce(() => Draw());
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
