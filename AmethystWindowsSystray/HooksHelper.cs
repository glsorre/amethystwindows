using DesktopMonitorManager.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
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
        public DesktopMonitorManager DesktopMonitorManager { get; set; }

        private DesktopMonitor desktopMonitorStart;
        private DesktopWindow desktopWindowStart;

        private string hotkeysConfPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AmethystWindows", "hotkeys.json");

        public HooksHelper(DesktopMonitorManager desktopWindowsManager)
        {
            DesktopMonitorManager = desktopWindowsManager;
        }

        private void ManageShown(HWND hWND)
        {
            DesktopWindow desktopWindow = new DesktopWindow(hWND);
            desktopWindow.GetInfo();
            if (desktopWindow != null && desktopWindow.IsRuntimePresent())
            {
                SystrayContext.Logger.Information($"window created");
                DesktopMonitorManager.AddWindow(desktopWindow);
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
                            ManageShown(hwnd);
                            break;
                        case User32.EventConstants.EVENT_SYSTEM_MINIMIZEEND:
                            SystrayContext.Logger.Information($"window maximized");
                            desktopWindow.GetInfo();
                            if (desktopWindow.IsRuntimePresent()) DesktopMonitorManager.AddWindow(desktopWindow);
                            break;
                        case User32.EventConstants.EVENT_SYSTEM_MINIMIZESTART:
                        case User32.EventConstants.EVENT_OBJECT_HIDE:
                        case User32.EventConstants.EVENT_OBJECT_IME_HIDE:
                            SystrayContext.Logger.Information($"window minimized");
                            DesktopWindow minimized = DesktopMonitorManager.FindWindow(hwnd);
                            if (minimized != null) DesktopMonitorManager.RemoveWindow(minimized);
                            break;
                        case User32.EventConstants.EVENT_OBJECT_DESTROY:
                            SystrayContext.Logger.Information($"window destroyed");
                            List<DesktopWindow> destroyed = new List<DesktopWindow>();
                            DesktopWindow main = DesktopMonitorManager.FindWindow(hwnd);
                            destroyed.Add(main); 
                            foreach (var desktopMonitor in DesktopMonitorManager.DesktopMonitors) {
                                foreach (var w in desktopMonitor.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)).Where((value) => !(value is null)))
                                {
                                    if (!w.Item2.IsRuntimePresent()) destroyed.Add(w.Item2);
                                }
                            }
                            if (destroyed.Count == 1)
                            {
                                if (destroyed[0] != null) DesktopMonitorManager.RemoveWindow(destroyed[0]);
                            } 
                            else if (destroyed.Count > 1)
                            {
                                DesktopMonitorManager.RemoveWindows(destroyed.Where((value) => value != null).ToList());
                            }
                            break;
                        case User32.EventConstants.EVENT_SYSTEM_MOVESIZESTART:
                            SystrayContext.Logger.Information($"window move/size start");
                            desktopWindowStart = DesktopMonitorManager.FindWindow(hwnd);
                            desktopMonitorStart = DesktopMonitorManager.FindDesktopMonitor(desktopWindowStart);
                            break;
                        case User32.EventConstants.EVENT_SYSTEM_MOVESIZEEND:
                            SystrayContext.Logger.Information($"window move/size end");
                            DesktopWindow desktopWindowEnd = DesktopMonitorManager.FindWindow(hwnd);
                            DesktopMonitor desktopMonitorEnd = desktopWindowEnd.GetDesktopMonitor();
                            if (!desktopMonitorStart.Equals(desktopMonitorEnd))
                            {
                                desktopMonitorStart.Remove(desktopWindowStart);
                                DesktopMonitorManager.AddWindow(desktopWindowEnd, desktopMonitorEnd);
                            }
                            break;
                        case User32.EventConstants.EVENT_OBJECT_DRAGCOMPLETE:
                            SystrayContext.Logger.Information($"window dragged");
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

        private List<Hotkey> LoadJson()
        {
            using (StreamReader r = new StreamReader(hotkeysConfPath))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<Hotkey>>(json);
            }
        }

        private enum HotkeyId : int
        {
            RotateLayoutsClockwise = 0x11,
            SetMainPane = 0x12,
            SwapFocusedCounterclockwise = 0x13,
            SwapFocusedClockwise = 0x14,
            ChangeFocusCounterclockwise = 0x15,
            ChangeFocusClockwise = 0x16,
            ForceWindowsEvaluation = 0x17,
            SelectNextScreen = 0x18,
            SelectPreviousScreen = 0x19,

            RotateLayoutsCounterClockwise = 0x21,
            ShrinkMainPane = 0x22,
            ExpandMainPane = 0x23,
            MoveFocusedToNextScreen = 0x24,
            MoveFocusedToPreviousScreen= 0x25,
            MoveFocusedToNextSpace= 0x26,

            MoveFocusedToSpace1 = 0x1,
            MoveFocusedToSpace2 = 0x2,
            MoveFocusedToPreviousSpace = 0x27,
            MoveFocusedToSpace3 = 0x3,
            MoveFocusedToSpace4 = 0x4,
            MoveFocusedToSpace5 = 0x5,
        }

        private class Hotkey
        {
            public HotkeyId id;
            public List<User32.HotKeyModifiers> modifier;
            public string key;
        }

        public void setKeyboardHook(HWND hWND)
        {
            if (File.Exists(hotkeysConfPath))
            {
                List<Hotkey> hotkeys = LoadJson();

                hotkeys.ForEach(hotkey =>
                {
                    User32.RegisterHotKey(
                        hWND,
                        (int)hotkey.id,
                        hotkey.modifier.Aggregate((a, b) => a | b),
                        (uint)(int)new System.ComponentModel.Int32Converter().ConvertFromString(hotkey.key)
                        );
                });

            } else {
                User32.RegisterHotKey(hWND, 0x11, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x20); //space
                User32.RegisterHotKey(hWND, 0x12, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x0D); //enter
                User32.RegisterHotKey(hWND, 0x13, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x48); //H
                User32.RegisterHotKey(hWND, 0x16, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x4C); //L
                User32.RegisterHotKey(hWND, 0x14, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x4A); //J 
                User32.RegisterHotKey(hWND, 0x15, User32.HotKeyModifiers.MOD_SHIFT | User32.HotKeyModifiers.MOD_ALT, 0x4B); //K
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
}
