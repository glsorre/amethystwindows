using DesktopMonitorManager.Internal;
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
    partial class DesktopMonitorManager
    {
        public void AddWindow(DesktopWindow desktopWindow)
        {
            List<Pair<string, string>> configurableFilter = ConfigurableFilters.Where(f => f.Item1 == desktopWindow.AppName).ToList();

            DesktopMonitor desktopMonitor = desktopWindow.GetDesktopMonitor();
            desktopMonitor = FindDesktopMonitor(desktopMonitor);

            if (desktopMonitor is null)
            {
                DesktopMonitors.Add(desktopMonitor);
                desktopMonitor.Subscribe(true, DesktopMonitor_CollectionChanged);
            }

            bool desktopWindowAlreadyAdded = desktopMonitor.Contains(desktopWindow);

            if (FixedFilters.All(s => !desktopWindow.AppName.StartsWith(s)) &&
                desktopWindow.AppName != String.Empty &&
                !desktopWindowAlreadyAdded &&
                !(desktopWindow is null))
            {
                if (configurableFilter.Count == 0)
                {
                    desktopMonitor.Add(desktopWindow);
                }
                else
                {
                    bool filtered = false;

                    foreach (Pair<string, string> filter in configurableFilter)
                    {
                        if (filter.Item2 == "*" || filter.Item2 == desktopWindow.ClassName)
                        {
                            filtered = true;
                        }
                    }

                    if (!filtered) desktopMonitor.Add(desktopWindow);
                }
            }
        }

        public void AddWindow(DesktopWindow desktopWindow, DesktopMonitor desktopMonitor)
        {
            Pair<string, string> configurableFilter = ConfigurableFilters.FirstOrDefault(f => f.Item1 == desktopWindow.AppName);

            DesktopMonitor existingDesktopMonitor = FindDesktopMonitor(desktopMonitor);

            if (existingDesktopMonitor is null)
            {
                DesktopMonitors.Add(desktopMonitor);
                desktopMonitor.Subscribe(true, DesktopMonitor_CollectionChanged);
            } else
            {
                desktopMonitor = existingDesktopMonitor;
            }

            bool desktopWindowAlreadyAdded = desktopMonitor.Contains(desktopWindow);

            if (FixedFilters.All(s => !desktopWindow.AppName.StartsWith(s)) &&
                desktopWindow.AppName != String.Empty &&
                !desktopWindowAlreadyAdded &&
                !(desktopWindow is null))
            {
                if (configurableFilter.Equals(null))
                {
                    desktopMonitor.Add(desktopWindow);
                }
                else
                {
                    if (configurableFilter.Item2 != "*" && configurableFilter.Item2 != desktopWindow.ClassName)
                    {
                        desktopMonitor.Add(desktopWindow);
                    }
                }
            }
        }

        public void RemoveWindow(DesktopWindow desktopWindow)
        {
            DesktopMonitor desktopMonitor = FindDesktopMonitor(desktopWindow);
            if (desktopMonitor != null) desktopMonitor.Remove(desktopWindow);
        }

        public void RemoveWindows(List<DesktopWindow> windows)
        {
            foreach (var desktopWindow in windows)
            {
                RemoveWindow(desktopWindow);
            }
        }

        public void ClearWindows()
        {
            foreach (var desktopMonitor in DesktopMonitors)
            {
                desktopMonitor.Clear();
            }
        }

        public void GetWindows()
        {
            User32.EnumWindowsProc filterDesktopWindows = delegate (HWND windowHandle, IntPtr lparam)
            {
                DesktopWindow desktopWindow = new DesktopWindow(windowHandle);

                if (desktopWindow.IsRuntimePresent())
                {
                    desktopWindow.GetInfo();
                    User32.ShowWindow(windowHandle, ShowWindowCommand.SW_RESTORE);
                    DesktopMonitor desktopMonitor = FindDesktopMonitor(desktopWindow.GetDesktopMonitor());

                    if (desktopMonitor is null)
                    {
                        desktopMonitor = desktopWindow.GetDesktopMonitor();
                        DesktopMonitors.Add(desktopMonitor);
                        AddWindow(desktopWindow, desktopMonitor);
                        
                    }
                    else
                    {
                        if (!desktopMonitor.Contains(desktopWindow))
                        {
                            AddWindow(desktopWindow, desktopMonitor);
                        }
                    }
                }
                return true;
            };

            User32.EnumWindows(filterDesktopWindows, IntPtr.Zero);

            foreach (var desktopMonitor in DesktopMonitors)
            {
                desktopMonitor.Subscribe(true, DesktopMonitor_CollectionChanged);
            }
        }

        private void DesktopMonitor_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action.Equals(NotifyCollectionChangedAction.Remove))
            {
                DesktopMonitor desktopMonitor = (DesktopMonitor)sender;
                Draw(desktopMonitor);
                DebounceDispatcherRemove.Debounce(() =>
                {
                    Changed.Invoke(this, "remove");
                });
            }
            else if (e.Action.Equals(NotifyCollectionChangedAction.Add))
            {
                DesktopMonitor desktopMonitor = (DesktopMonitor)sender;
                Draw(desktopMonitor);
                DebounceDispatcherAdd.Debounce(() =>
                {
                    Changed.Invoke(this, "add");
                });
            }
        }

        public DesktopWindow FindWindow(HWND hWND)
        {
            List<DesktopWindow> desktopWindows = new List<DesktopWindow>();
            foreach (var desktopMonitor in DesktopMonitors)
            {
                desktopWindows.AddRange(desktopMonitor.Where(window => window.Window == hWND));
            }
            return desktopWindows.FirstOrDefault();
        }

        public DesktopWindow GetWindowByHandlers(HWND hWND, HMONITOR monitor, VirtualDesktop desktop)
        {

            return FindDesktopMonitor(monitor, desktop).FirstOrDefault(window => window.Window == hWND);
        }

        public void SetMainWindow(DesktopMonitor desktopMonitor, DesktopWindow window)
        {
            FindDesktopMonitor(desktopMonitor).Move(
                    desktopMonitor.IndexOf(window),
                    0
                    );
        }

        public void RotateFocusedWindowClockwise(DesktopMonitor desktopMonitor, DesktopWindow window)
        {
            desktopMonitor = FindDesktopMonitor(desktopMonitor);
            int currentIndex = desktopMonitor.IndexOf(window);
            int maxIndex = desktopMonitor.Count - 1;
            if (currentIndex == maxIndex)
            {
                User32.SetForegroundWindow(desktopMonitor[0].Window);
            }
            else
            {
                User32.SetForegroundWindow(desktopMonitor[++currentIndex].Window);
            }
        }

        public void RotateFocusedWindowCounterClockwise(DesktopMonitor desktopMonitor, DesktopWindow window)
        {
            desktopMonitor = FindDesktopMonitor(desktopMonitor);
            int currentIndex = desktopMonitor.IndexOf(window);
            int maxIndex = desktopMonitor.Count - 1;
            if (currentIndex == 0)
            {
                User32.SetForegroundWindow(desktopMonitor[maxIndex].Window);
            }
            else
            {
                User32.SetForegroundWindow(desktopMonitor[--currentIndex].Window);
            }
        }

        public void MoveWindowClockwise(DesktopMonitor desktopMonitor, DesktopWindow window)
        {
            desktopMonitor = FindDesktopMonitor(desktopMonitor);
            int currentIndex = desktopMonitor.IndexOf(window);
            int maxIndex = desktopMonitor.Count - 1;
            if (currentIndex == maxIndex)
            {
                desktopMonitor.Move(currentIndex, 0);
            }
            else
            {
                desktopMonitor.Move(currentIndex, ++currentIndex);
            }
        }

        public void MoveWindowCounterClockwise(DesktopMonitor desktopMonitor, DesktopWindow window)
        {
            desktopMonitor = FindDesktopMonitor(desktopMonitor);
            int currentIndex = desktopMonitor.IndexOf(window);
            int maxIndex = desktopMonitor.Count - 1;
            if (currentIndex == 0)
            {
                desktopMonitor.Move(currentIndex, maxIndex);
            }
            else
            {
                desktopMonitor.Move(currentIndex, --currentIndex);
            }
        }

        public void MoveWindowNextScreen(DesktopWindow window)
        {
            List<DesktopMonitor> desktopMonitors = DesktopMonitors.Where(dM => dM.VirtualDesktop == VirtualDesktop.Current).ToList();
            int currentMonitorIndex = desktopMonitors.IndexOf(window.GetDesktopMonitor());
            int maxIndex = desktopMonitors.Count - 1;
            if (desktopMonitors.Count > 1)
            {
                desktopMonitors[currentMonitorIndex].Remove(window);
                if (currentMonitorIndex == maxIndex)
                {
                    desktopMonitors[0].Add(window);
                }
                else
                {
                    desktopMonitors[++currentMonitorIndex].Add(window);
                }
            }
        }

        public void MoveWindowPreviousScreen(DesktopWindow window)
        {
            List<DesktopMonitor> desktopMonitors = DesktopMonitors.Where(dM => dM.VirtualDesktop == VirtualDesktop.Current).ToList();
            int currentMonitorIndex = desktopMonitors.IndexOf(window.GetDesktopMonitor());
            int maxIndex = desktopMonitors.Count - 1;
            if (desktopMonitors.Count > 1)
            {
                desktopMonitors[currentMonitorIndex].Remove(window);
                if (currentMonitorIndex == 0)
                {
                    desktopMonitors[maxIndex].Add(window);
                }
                else
                {
                    desktopMonitors[--currentMonitorIndex].Add(window);
                }
            }
        }

        public void MoveWindowNextVirtualDesktop(DesktopWindow window)
        {
            DesktopMonitor desktopMonitor = window.GetDesktopMonitor();
            VirtualDesktop nextVirtualDesktop = desktopMonitor.VirtualDesktop.Right;
            DesktopMonitor nextDesktopMonitor = new DesktopMonitor(window.GetDesktopMonitor().Monitor, nextVirtualDesktop);
            DesktopMonitor existingNextDesktopMonitor = FindDesktopMonitor(nextDesktopMonitor);
            if (existingNextDesktopMonitor is null)
            {
                DesktopMonitors.Add(nextDesktopMonitor);
            }
            desktopMonitor.Remove(window);
            nextVirtualDesktop.MoveWindow(window.Window);
            nextDesktopMonitor.Add(window);
            nextVirtualDesktop.MakeVisible();
        }

        public void MoveWindowPreviousVirtualDesktop(DesktopWindow window)
        {
            DesktopMonitor desktopMonitor = window.GetDesktopMonitor();
            VirtualDesktop nextVirtualDesktop = desktopMonitor.VirtualDesktop.Left;
            DesktopMonitor nextDesktopMonitor = new DesktopMonitor(window.GetDesktopMonitor().Monitor, nextVirtualDesktop);
            DesktopMonitor existingNextDesktopMonitor = FindDesktopMonitor(nextDesktopMonitor);
            if (existingNextDesktopMonitor is null)
            {
                DesktopMonitors.Add(nextDesktopMonitor);
            }
            desktopMonitor.Remove(window);
            nextVirtualDesktop.MoveWindow(window.Window);
            nextDesktopMonitor.Add(window);
            nextVirtualDesktop.MakeVisible();
        }

        public void MoveWindowSpecificVirtualDesktop(DesktopWindow window, int id)
        {
            DesktopMonitor desktopMonitor = window.GetDesktopMonitor();
            VirtualDesktop nextVirtualDesktop = VirtualDesktop.FromIndex(id);
            DesktopMonitor nextDesktopMonitor = new DesktopMonitor(window.GetDesktopMonitor().Monitor, nextVirtualDesktop);
            DesktopMonitor existingNextDesktopMonitor = FindDesktopMonitor(nextDesktopMonitor);
            if (existingNextDesktopMonitor is null)
            {
                DesktopMonitors.Add(nextDesktopMonitor);
            }
            desktopMonitor.Remove(window);
            nextVirtualDesktop.MoveWindow(window.Window);
            nextDesktopMonitor.Add(window);
            nextVirtualDesktop.MakeVisible();
        }
    }
}
