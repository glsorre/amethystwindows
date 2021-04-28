using DesktopWindowManager.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Vanara.PInvoke;
using WindowsDesktop;

namespace AmethystWindowsSystray
{
    class HooksHelper
    {
        public DesktopWindowsManager DesktopWindowsManager { get; set; }
        

        public HooksHelper(DesktopWindowsManager desktopWindowsManager)
        {
            DesktopWindowsManager = desktopWindowsManager;
        }

        private async Task ManageShown(HWND hWND)
        {
            await Task.Delay(500);
            DesktopWindow desktopWindow = new DesktopWindow(hWND);
            desktopWindow.GetInfo();
            if (desktopWindow.IsRuntimePresent())
            {
                SystrayContext.Logger.Information($"window created");
                DesktopWindowsManager.AddWindow(desktopWindow);
            }
        }

        public void setWindowsHook()
        {
            void WinEventHookAll(User32.HWINEVENTHOOK hWinEventHook, uint winEvent, HWND hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
            {
               
                DesktopWindow desktopWindow = new DesktopWindow(hwnd);
                if (hwnd != HWND.NULL && idObject == User32.ObjectIdentifiers.OBJID_WINDOW && idChild == 0 && desktopWindow.IsRuntimeValuable())
                {
                    switch (winEvent)
                    {
                        case User32.EventConstants.EVENT_OBJECT_SHOW:
                        case User32.EventConstants.EVENT_OBJECT_UNCLOAKED:
                        case User32.EventConstants.EVENT_OBJECT_IME_SHOW:
                        case User32.EventConstants.EVENT_SYSTEM_FOREGROUND:
                            ManageShown(hwnd);
                            break;
                        case User32.EventConstants.EVENT_SYSTEM_MINIMIZEEND:
                            SystrayContext.Logger.Information($"window maximized");
                            desktopWindow.GetInfo();
                            DesktopWindowsManager.AddWindow(desktopWindow);
                            break;
                        case User32.EventConstants.EVENT_SYSTEM_MINIMIZESTART:
                        case User32.EventConstants.EVENT_OBJECT_HIDE:
                        case User32.EventConstants.EVENT_OBJECT_IME_HIDE:
                            SystrayContext.Logger.Information($"window minimized/hide");
                            HMONITOR monitorHandle = User32.MonitorFromWindow(hwnd, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                            DesktopWindow remove = DesktopWindowsManager.GetWindowByHandlers(hwnd, monitorHandle, VirtualDesktop.Current);
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

            User32.WinEventProc winEventHookAll = new User32.WinEventProc(WinEventHookAll);
            GCHandle gchCreate = GCHandle.Alloc(winEventHookAll);

            User32.HWINEVENTHOOK hookAll = User32.SetWinEventHook(User32.EventConstants.EVENT_MIN, User32.EventConstants.EVENT_MAX, HINSTANCE.NULL, winEventHookAll, 0, 0, User32.WINEVENT.WINEVENT_OUTOFCONTEXT | User32.WINEVENT.WINEVENT_SKIPOWNPROCESS);
        }

        public void setKeyboardHook(HWND hWND)
        {
            User32.RegisterHotKey(hWND, 0x11, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x20); //space
            User32.RegisterHotKey(hWND, 0x12, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x0D); //enter
            User32.RegisterHotKey(hWND, 0x13, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x48); //H
            User32.RegisterHotKey(hWND, 0x14, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x4A); //J 
            User32.RegisterHotKey(hWND, 0x15, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x4B); //K
            User32.RegisterHotKey(hWND, 0x16, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x4C); //L
            User32.RegisterHotKey(hWND, 0x17, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x5A); //Z
            User32.RegisterHotKey(hWND, 0x18, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x50); //P
            User32.RegisterHotKey(hWND, 0x19, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x4E); //N

            User32.RegisterHotKey(hWND, 0x21, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT | User32.HotKeyModifiers.MOD_WIN, 0x20); //space
            User32.RegisterHotKey(hWND, 0x22, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT | User32.HotKeyModifiers.MOD_WIN, 0x48); //H
            User32.RegisterHotKey(hWND, 0x23, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT | User32.HotKeyModifiers.MOD_WIN, 0x4C); //L
            User32.RegisterHotKey(hWND, 0x24, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT | User32.HotKeyModifiers.MOD_WIN, 0x4A); //J
            User32.RegisterHotKey(hWND, 0x25, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT | User32.HotKeyModifiers.MOD_WIN, 0x4B); //K
            User32.RegisterHotKey(hWND, 0x26, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT | User32.HotKeyModifiers.MOD_WIN, 0x27); //right
            User32.RegisterHotKey(hWND, 0x27, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT | User32.HotKeyModifiers.MOD_WIN, 0x25); //left

            User32.RegisterHotKey(hWND, 0x1, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT | User32.HotKeyModifiers.MOD_WIN, 0x31); //1
            User32.RegisterHotKey(hWND, 0x2, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT | User32.HotKeyModifiers.MOD_WIN, 0x32); //2
            User32.RegisterHotKey(hWND, 0x3, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT | User32.HotKeyModifiers.MOD_WIN, 0x33); //3
            User32.RegisterHotKey(hWND, 0x4, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT | User32.HotKeyModifiers.MOD_WIN, 0x34); //4
            User32.RegisterHotKey(hWND, 0x5, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT | User32.HotKeyModifiers.MOD_WIN, 0x35); //5
        }
    }
}
