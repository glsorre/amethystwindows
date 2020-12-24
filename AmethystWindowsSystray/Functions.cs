using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using WindowsDesktop;

namespace AmethystWindowsSystray
{
    class Functions
    {
        public DesktopWindowsManager DesktopWindowsManager { get; set; }

        public Functions(DesktopWindowsManager desktopWindowsManager)
        {
            DesktopWindowsManager = desktopWindowsManager;
        }

        public void GetWindows()
        {
            User32.EnumWindowsProc filterDesktopWindows = delegate (HWND windowHandle, IntPtr lparam)
            {
                DesktopWindow desktopWindow = new DesktopWindow(windowHandle);

                if (desktopWindow.isPresent())
                {
                    desktopWindow.GetAppName();
                    desktopWindow.GetMonitorInfo();
                    desktopWindow.GetVirtualDesktop();
                    try
                    {
                        if (!DesktopWindowsManager.Windows[desktopWindow.GetDesktopMonitor()].Contains(desktopWindow))
                        {
                            DesktopWindowsManager.Windows[desktopWindow.GetDesktopMonitor()].Add(desktopWindow);
                        }
                    }
                    catch
                    {
                        DesktopWindowsManager.Windows.Add(
                            desktopWindow.GetDesktopMonitor(),
                            new ObservableCollection<DesktopWindow>(new DesktopWindow[] { desktopWindow })
                            );
                        
                        DesktopWindowsManager.Windows[desktopWindow.GetDesktopMonitor()].CollectionChanged += Functions_CollectionChanged;
                    }

                }
                return true;
            };

            User32.EnumWindows(filterDesktopWindows, IntPtr.Zero);
        }

        private void Functions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action.Equals(NotifyCollectionChangedAction.Remove))
            {
                DesktopWindow desktopWindow = (DesktopWindow)e.OldItems[0];
                DesktopWindowsManager.Draw(desktopWindow);
            }
            else if (e.Action.Equals(NotifyCollectionChangedAction.Add))
            {
                DesktopWindow desktopWindow = (DesktopWindow)e.NewItems[0];
                DesktopWindowsManager.Draw(desktopWindow);
            }
        }

        public void GetLayouts()
        {
            foreach (Pair<VirtualDesktop, HMONITOR> desktopMonitor in DesktopWindowsManager.Windows.Keys)
            {
                DesktopWindowsManager.Layouts.Clear();
                DesktopWindowsManager.Layouts.Add(desktopMonitor, Layout.Horizontal);
            }
        }

        private async Task ManageCreated(DesktopWindow desktopWindow)
        {
            await Task.Delay(1000);
            if (desktopWindow.isRuntimeValuable())
            {
                desktopWindow.GetInfo();
                DesktopWindowsManager.AddWindow(desktopWindow);
            }
        }

        public void setWindowsHook()
        {
            void WinEventHookAll(User32.HWINEVENTHOOK hWinEventHook, uint winEvent, HWND hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
            {
                DesktopWindow desktopWindow = new DesktopWindow(hwnd);
                HWND shellWindow = User32.GetShellWindow();

                if (hwnd != HWND.NULL && idObject == User32.ObjectIdentifiers.OBJID_WINDOW && idChild == 0 && desktopWindow.isRuntimeValuable())
                {
                    switch (winEvent)
                    {
                        case User32.EventConstants.EVENT_SYSTEM_MINIMIZEEND:
                            SystrayContext.Logger.Information($"window maximized");
                            desktopWindow.GetInfo();
                            DesktopWindowsManager.AddWindow(desktopWindow);
                            break;
                        case User32.EventConstants.EVENT_SYSTEM_MINIMIZESTART:
                        case User32.EventConstants.EVENT_OBJECT_HIDE:
                            SystrayContext.Logger.Information($"window minimized/hide");
                            HMONITOR removedMonitorHandle = User32.MonitorFromWindow(hwnd, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                            DesktopWindow remove = DesktopWindowsManager.GetWindowByHandlers(hwnd, removedMonitorHandle, VirtualDesktop.Current);
                            if (remove != null)DesktopWindowsManager.RemoveWindow(remove);
                            break;
                        case User32.EventConstants.EVENT_SYSTEM_MOVESIZEEND:
                            SystrayContext.Logger.Information($"window move/size");
                            VirtualDesktop movedDesktop = VirtualDesktop.FromHwnd(hwnd);
                            DesktopWindow moved = DesktopWindowsManager.FindWindow(hwnd);
                            if (moved != null)
                            {
                                DesktopWindow newMoved = new DesktopWindow(hwnd);
                                newMoved.GetInfo();
                                if (!moved.Equals(newMoved))
                                {
                                    DesktopWindowsManager.RepositionWindow(moved, newMoved);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            void WinEventHookCreate(User32.HWINEVENTHOOK hWinEventHook, uint winEvent, HWND hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
            {
                DesktopWindow desktopWindow = new DesktopWindow(hwnd);

                if (hwnd != HWND.NULL && idObject == User32.ObjectIdentifiers.OBJID_WINDOW && idChild == 0)
                {
                    switch (winEvent)
                    {
                        case User32.EventConstants.EVENT_OBJECT_CREATE:
                            SystrayContext.Logger.Information($"window created");
                            ManageCreated(desktopWindow);
                            break;
                        default:
                            break;
                    }
                }
            }

            User32.WinEventProc winEventHookCreate = new User32.WinEventProc(WinEventHookCreate);
            GCHandle gchAll = GCHandle.Alloc(winEventHookCreate);

            User32.WinEventProc winEventHookAll = new User32.WinEventProc(WinEventHookAll);
            GCHandle gchCreate = GCHandle.Alloc(winEventHookAll);

            User32.HWINEVENTHOOK hookAll = User32.SetWinEventHook(User32.EventConstants.EVENT_MIN, User32.EventConstants.EVENT_MAX, HINSTANCE.NULL, winEventHookAll, 0, 0, User32.WINEVENT.WINEVENT_OUTOFCONTEXT | User32.WINEVENT.WINEVENT_SKIPOWNPROCESS);
            User32.HWINEVENTHOOK hookCreate = User32.SetWinEventHook(User32.EventConstants.EVENT_OBJECT_CREATE, User32.EventConstants.EVENT_OBJECT_CREATE, HINSTANCE.NULL, winEventHookCreate, 0, 0, User32.WINEVENT.WINEVENT_OUTOFCONTEXT | User32.WINEVENT.WINEVENT_SKIPOWNPROCESS);
        }

        public void setKeyboardHook(HWND hWND)
        {
            User32.RegisterHotKey(hWND, 0x20, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x20);
            User32.RegisterHotKey(hWND, 0x0D, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x0D);
            User32.RegisterHotKey(hWND, 0x48, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x48);
            User32.RegisterHotKey(hWND, 0x4A, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x4A);
            User32.RegisterHotKey(hWND, 0x4B, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x4B);
            User32.RegisterHotKey(hWND, 0x4C, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x4C);
        }
    }
}
