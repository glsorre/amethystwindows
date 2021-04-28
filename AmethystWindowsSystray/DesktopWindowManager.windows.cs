﻿using DesktopWindowManager.Internal;
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

            if (FixedFilters.All(s => !desktopWindow.AppName.StartsWith(s)) 
                && desktopWindow.AppName != String.Empty &&
                !Windows[desktopWindow.GetDesktopMonitor()].Contains(desktopWindow))
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
            Windows[desktopWindow.GetDesktopMonitor()].Remove(desktopWindow);
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

                if (desktopWindow.IsRuntimePresent())
                {
                    User32.ShowWindow(windowHandle, ShowWindowCommand.SW_RESTORE);
                    desktopWindow.GetInfo();

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

        public void SetMainWindow(Pair<VirtualDesktop, HMONITOR> desktopMonitor, DesktopWindow window)
        {
            Windows[desktopMonitor].Move(
                    Windows[desktopMonitor].IndexOf(window),
                    0
                    );
        }

        public void RotateFocusedWindowClockwise(Pair<VirtualDesktop, HMONITOR> desktopMonitor, DesktopWindow window)
        {
            int currentIndex = Windows[desktopMonitor].IndexOf(window);
            int maxIndex = Windows[desktopMonitor].Count - 1;
            if (currentIndex == maxIndex)
            {
                User32.SetForegroundWindow(Windows[desktopMonitor][0].Window);
            }
            else
            {
                User32.SetForegroundWindow(Windows[desktopMonitor][++currentIndex].Window);
            }
        }

        public void RotateFocusedWindowCounterClockwise(Pair<VirtualDesktop, HMONITOR> desktopMonitor, DesktopWindow window)
        {
            int currentIndex = Windows[desktopMonitor].IndexOf(window);
            int maxIndex = Windows[desktopMonitor].Count - 1;
            if (currentIndex == 0)
            {
                User32.SetForegroundWindow(Windows[desktopMonitor][maxIndex].Window);
            }
            else
            {
                User32.SetForegroundWindow(Windows[desktopMonitor][--currentIndex].Window);
            }
        }

        public void MoveWindowClockwise(Pair<VirtualDesktop, HMONITOR> desktopMonitor, DesktopWindow window)
        {
            int currentIndex = Windows[desktopMonitor].IndexOf(window);
            int maxIndex = Windows[desktopMonitor].Count - 1;
            if (currentIndex == maxIndex)
            {
                Windows[desktopMonitor].Move(currentIndex, 0);
            }
            else
            {
                Windows[desktopMonitor].Move(currentIndex, ++currentIndex);
            }
        }

        public void MoveWindowCounterClockwise(Pair<VirtualDesktop, HMONITOR> desktopMonitor, DesktopWindow window)
        {
            int currentIndex = Windows[desktopMonitor].IndexOf(window);
            int maxIndex = Windows[desktopMonitor].Count - 1;
            if (currentIndex == 0)
            {
                Windows[desktopMonitor].Move(currentIndex, maxIndex);
            }
            else
            {
                Windows[desktopMonitor].Move(currentIndex, --currentIndex);
            }
        }

        public void MoveWindowNextScreen(DesktopWindow window)
        {
            List<Pair<VirtualDesktop, HMONITOR>> desktopMonitors = Windows.Keys.Where(dM => dM.Item1.ToString() == VirtualDesktop.Current.ToString()).ToList();
            int currentMonitorIndex = desktopMonitors.IndexOf(window.GetDesktopMonitor());
            int maxIndex = desktopMonitors.Count - 1;
            if (currentMonitorIndex == maxIndex)
            {
                RemoveWindow(window);
                window.MonitorHandle = desktopMonitors[0].Item2;
                AddWindow(window);
            }
            else
            {
                RemoveWindow(window);
                window.MonitorHandle = desktopMonitors[++currentMonitorIndex].Item2;
                AddWindow(window);
            }
        }

        public void MoveWindowPreviousScreen(DesktopWindow window)
        {
            List<Pair<VirtualDesktop, HMONITOR>> desktopMonitors = Windows.Keys.Where(dM => dM.Item1.ToString() == VirtualDesktop.Current.ToString()).ToList();
            int currentMonitorIndex = desktopMonitors.IndexOf(window.GetDesktopMonitor());
            int maxIndex = desktopMonitors.Count - 1;
            if (currentMonitorIndex == 0)
            {
                RemoveWindow(window);
                window.MonitorHandle = desktopMonitors[maxIndex].Item2;
                AddWindow(window);
            }
            else
            {
                RemoveWindow(window);
                window.MonitorHandle = desktopMonitors[--currentMonitorIndex].Item2;
                AddWindow(window);
            }
        }

        public void MoveWindowNextVirtualDesktop(DesktopWindow window)
        {
            VirtualDesktop nextVirtualDesktop = window.VirtualDesktop.Right;
            if (nextVirtualDesktop != null)
            {
                RemoveWindow(window);
                nextVirtualDesktop.MoveWindow(window.Window);
                window.VirtualDesktop = nextVirtualDesktop;
                AddWindow(window);
                nextVirtualDesktop.MakeVisible();
            }
        }

        public void MoveWindowPreviousVirtualDesktop(DesktopWindow window)
        {
            VirtualDesktop nextVirtualDesktop = window.VirtualDesktop.Left;
            if (nextVirtualDesktop != null) 
            {
                RemoveWindow(window);
                nextVirtualDesktop.MoveWindow(window.Window);
                window.VirtualDesktop = nextVirtualDesktop;
                AddWindow(window);
                nextVirtualDesktop.MakeVisible();
            }
        }

        public void MoveWindowSpecificVirtualDesktop(DesktopWindow window, int id)
        {
            VirtualDesktop nextVirtualDesktop = VirtualDesktop.FromIndex(id);
            if (nextVirtualDesktop != null)
            {
                RemoveWindow(window);
                nextVirtualDesktop.MoveWindow(window.Window);
                window.VirtualDesktop = nextVirtualDesktop;
                AddWindow(window);
                nextVirtualDesktop.MakeVisible();
            }
        }
    }
}
