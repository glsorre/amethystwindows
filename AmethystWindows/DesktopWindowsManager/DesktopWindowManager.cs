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

namespace AmethystWindows.DesktopWindowsManager
{
    partial class DesktopWindowsManager
    {
        private Dictionary<Pair<VirtualDesktop, HMONITOR>, Layout> Layouts;
        public Dictionary<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>> Windows { get; }
        public Dictionary<Pair<VirtualDesktop, HMONITOR>, bool> WindowsSubscribed = new Dictionary<Pair<VirtualDesktop, HMONITOR>, bool>();

        private Dictionary<Pair<VirtualDesktop, HMONITOR>, int> Factors;

        public MainWindowViewModel mainWindowViewModel = App.Current.MainWindow.DataContext as MainWindowViewModel;

        private DebounceDispatcher debounceDispatcher = new DebounceDispatcher(250);

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
            "Windows",
        };

        public DesktopWindowsManager()
        {
            Layouts = new Dictionary<Pair<VirtualDesktop, HMONITOR>, Layout>();
            Windows = new Dictionary<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>>();
            Factors = new Dictionary<Pair<VirtualDesktop, HMONITOR>, int>();

            mainWindowViewModel.PropertyChanged += MainWindowViewModel_PropertyChanged;
        }

        private void MainWindowViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ConfigurableFilters")
            {
                ClearWindows();
                CollectWindows();
            }
            if (ModelViewPropertiesDraw.Contains(e.PropertyName))
            {
                debounceDispatcher.Debounce(() =>
                {
                    Draw();
                });
            }
        }

        public void RotateMonitorClockwise(Pair<VirtualDesktop, HMONITOR> currentDesktopMonitor)
        {
            List<HMONITOR> virtualDesktopMonitors = Windows
                .Keys
                .Where(desktopMonitor => desktopMonitor.Item1.Equals(currentDesktopMonitor.Item1))
                .Select(desktopMonitor => desktopMonitor.Item2)
                .ToList();

            HMONITOR nextMonitor = virtualDesktopMonitors.SkipWhile(x => x != currentDesktopMonitor.Item2).Skip(1).DefaultIfEmpty(virtualDesktopMonitors[0]).FirstOrDefault();
            Pair<VirtualDesktop, HMONITOR> nextDesktopMonitor = new Pair<VirtualDesktop, HMONITOR>(currentDesktopMonitor.Item1, nextMonitor);

            User32.SetForegroundWindow(Windows[nextDesktopMonitor].FirstOrDefault().Window);
        }

        public void RotateMonitorCounterClockwise(Pair<VirtualDesktop, HMONITOR> currentDesktopMonitor)
        {
            List<HMONITOR> virtualDesktopMonitors = Windows
                .Keys
                .Where(desktopMonitor => desktopMonitor.Item1.Equals(currentDesktopMonitor.Item1))
                .Select(desktopMonitor => desktopMonitor.Item2)
                .ToList();

            HMONITOR nextMonitor = virtualDesktopMonitors.TakeWhile(x => x != currentDesktopMonitor.Item2).Skip(1).DefaultIfEmpty(virtualDesktopMonitors[0]).FirstOrDefault();
            Pair<VirtualDesktop, HMONITOR> nextDesktopMonitor = new Pair<VirtualDesktop, HMONITOR>(currentDesktopMonitor.Item1, nextMonitor);

            User32.SetForegroundWindow(Windows[nextDesktopMonitor].FirstOrDefault().Window);
        }

        public void LoadFactors()
        {
            if (MySettings.Instance.Factors != "[]")
            {
                ReadFactors();
            }
        }

        public void SaveFactors()
        {
            MySettings.Instance.Factors = JsonConvert.SerializeObject(Factors.ToList(), Formatting.Indented, new FactorsConverter());
            MySettings.Save();
        }

        public void ReadFactors()
        {
            Factors = JsonConvert.DeserializeObject<List<KeyValuePair<Pair<VirtualDesktop, HMONITOR>, int>>>(
                MySettings.Instance.Factors, new FactorsConverter()
                ).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public void UpdateFactors()
        {
            foreach (Pair<VirtualDesktop, HMONITOR> desktopMonitor in Windows.Keys)
            {
                if (!Factors.ContainsKey(desktopMonitor))
                {
                    Factors.Add(desktopMonitor, 0);
                }
            }
        }

        private void SubscribeWindowsCollectionChanged(Pair<VirtualDesktop, HMONITOR> desktopMonitor, bool enabled)
        {
            if (!enabled) Windows[desktopMonitor].CollectionChanged -= Windows_CollectionChanged;
            else if (!WindowsSubscribed[desktopMonitor]) Windows[desktopMonitor].CollectionChanged += Windows_CollectionChanged;

            WindowsSubscribed[desktopMonitor] = enabled;
        }

    }
}
