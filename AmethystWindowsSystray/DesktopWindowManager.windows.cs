using DesktopWindowManager.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using WindowsDesktop;

[assembly: InternalsVisibleTo("AmethystWindowsSystrayTests")]
namespace AmethystWindowsSystray
{
    partial class DesktopWindowsManager
    {
        public void AddWindow(DesktopWindow desktopWindow)
        {
            Pair<string, string> configurableFilter = ConfigurableFilters.FirstOrDefault(f => f.Item1 == desktopWindow.AppName);

            if (!Windows.ContainsKey(desktopWindow.GetDesktopMonitor()))
            {
                Windows.Add(desktopWindow.GetDesktopMonitor(), new ObservableCollection<DesktopWindow>());
                Windows[desktopWindow.GetDesktopMonitor()].CollectionChanged += Windows_CollectionChanged;
            }

            if (!FixedFilters.Contains(desktopWindow.AppName))
            { 
                if (configurableFilter.Equals(null))
                {
                    Windows[desktopWindow.GetDesktopMonitor()].Add(desktopWindow);
                }
                else
                {
                    if (configurableFilter.Item2 != "*" && configurableFilter.Item2 != desktopWindow.ClassName)
                    {
                        Windows[desktopWindow.GetDesktopMonitor()].Add(desktopWindow);
                    }
                }
            }
        }

        public void RemoveWindow(DesktopWindow desktopWindow)
        {
            Windows[new Pair<VirtualDesktop, HMONITOR>(desktopWindow.VirtualDesktop, desktopWindow.MonitorHandle)].Remove(desktopWindow);
        }

        public void RepositionWindow(DesktopWindow oldDesktopWindow, DesktopWindow newDesktopWindow)
        {
            RemoveWindow(oldDesktopWindow);
            AddWindow(newDesktopWindow);
        }

        public void ClearWindows()
        {
            foreach (var desktopMonitor in Windows)
            {
                desktopMonitor.Value.Clear();
            }
        }

        public void GetWindows()
        {
            User32.EnumWindowsProc filterDesktopWindows = delegate (HWND windowHandle, IntPtr lparam)
            {
                DesktopWindow desktopWindow = new DesktopWindow(windowHandle);

                if (desktopWindow.isPresent())
                {
                    desktopWindow.GetAppName();
                    desktopWindow.GetClassName();
                    desktopWindow.GetMonitorInfo();
                    desktopWindow.GetVirtualDesktop();

                    if (Windows.ContainsKey(desktopWindow.GetDesktopMonitor()))
                    {
                        if (!Windows[desktopWindow.GetDesktopMonitor()].Contains(desktopWindow))
                        {
                            AddWindow(desktopWindow);
                        }
                    }
                    else
                    {
                        Windows.Add(
                            desktopWindow.GetDesktopMonitor(),
                            new ObservableCollection<DesktopWindow>(new DesktopWindow[] { })
                            );
                        AddWindow(desktopWindow);
                    }
                }
                return true;
            };

            User32.EnumWindows(filterDesktopWindows, IntPtr.Zero);

            foreach (var desktopMonitor in Windows)
            {
                Windows[desktopMonitor.Key].CollectionChanged += Windows_CollectionChanged;
            }
        }

        private void Windows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action.Equals(NotifyCollectionChangedAction.Remove))
            {
                DesktopWindow desktopWindow = (DesktopWindow)e.OldItems[0];
                Draw(desktopWindow.GetDesktopMonitor());
                Changed.Invoke(this, "add");
            }
            else if (e.Action.Equals(NotifyCollectionChangedAction.Add))
            {
                DesktopWindow desktopWindow = (DesktopWindow)e.NewItems[0];
                Draw(desktopWindow.GetDesktopMonitor());
                Changed.Invoke(this, "remove");
            }
        }

        public DesktopWindow FindWindow(HWND hWND)
        {
            List<DesktopWindow> desktopWindows = new List<DesktopWindow>();
            foreach (var desktopMonitor in Windows)
            {
                desktopWindows.AddRange(Windows[new Pair<VirtualDesktop, HMONITOR>(desktopMonitor.Key.Item1, desktopMonitor.Key.Item2)].Where(window => window.Window == hWND));
            }
            return desktopWindows.FirstOrDefault();
        }

        public DesktopWindow GetWindowByHandlers(HWND hWND, HMONITOR hMONITOR, VirtualDesktop desktop)
        {
            return Windows[new Pair<VirtualDesktop, HMONITOR>(desktop, hMONITOR)].FirstOrDefault(window => window.Window == hWND);
        }
    }
}
