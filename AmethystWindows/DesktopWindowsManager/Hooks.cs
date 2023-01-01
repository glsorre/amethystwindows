using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Vanara.PInvoke;

namespace AmethystWindows.DesktopWindowsManager
{
    internal class Hooks
    {
        public DesktopWindowsManager DesktopWindowsManager { get; set; }

        private ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        ILogger logger;

        public Hooks(DesktopWindowsManager desktopWindowsManager)
        {
            logger = loggerFactory.CreateLogger<Hooks>();
            DesktopWindowsManager = desktopWindowsManager;
        }

        private async void ManageShown(HWND hWND)
        {
            await Task.Delay(500);
            DesktopWindow desktopWindow = new DesktopWindow(hWND);
            desktopWindow.GetInfo();
            DesktopWindowsManager.mainWindowViewModel.LastChangedDesktopMonitor = desktopWindow.GetDesktopMonitor();
            Pair<string, string> configurableAddition = DesktopWindowsManager.mainWindowViewModel.ConfigurableAdditions.FirstOrDefault(f => f.Key == desktopWindow.AppName);
            bool hasActiveAddition;
            if (!(configurableAddition.Key == null))
            {
                hasActiveAddition = configurableAddition.Value.Equals("*") || configurableAddition.Value.Equals(desktopWindow.ClassName);
            } else
            {
                hasActiveAddition = false;
            }         
            if (desktopWindow.IsRuntimePresent() || hasActiveAddition)
            {
                Debug.WriteLine($"window created");
                DesktopWindowsManager.AddWindow(desktopWindow);
            } else
            {
                if (desktopWindow.IsExcluded() && !DesktopWindowsManager.ExcludedWindows.Contains(desktopWindow) && desktopWindow.AppName != "" && !DesktopWindowsManager.FixedExcludedFilters.Contains(desktopWindow.AppName)) DesktopWindowsManager.ExcludedWindows.Add(desktopWindow);
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
                            Debug.WriteLine($"window maximized");
                            desktopWindow.GetInfo();
                            DesktopWindowsManager.mainWindowViewModel.LastChangedDesktopMonitor = desktopWindow.GetDesktopMonitor();
                            Pair<string, string> configurableAddition = DesktopWindowsManager.mainWindowViewModel.ConfigurableAdditions.FirstOrDefault(f => f.Key == desktopWindow.AppName);
                            bool hasActiveAddition;
                            if (!(configurableAddition.Key == null))
                            {
                                hasActiveAddition = configurableAddition.Value.Equals("*") || configurableAddition.Value.Equals(desktopWindow.ClassName);
                            }
                            else
                            {
                                hasActiveAddition = false;
                            }
                            if (desktopWindow.IsRuntimePresent() || hasActiveAddition) DesktopWindowsManager.AddWindow(desktopWindow);
                            break;
                        case User32.EventConstants.EVENT_SYSTEM_MINIMIZESTART:
                        case User32.EventConstants.EVENT_OBJECT_HIDE:
                        case User32.EventConstants.EVENT_OBJECT_IME_HIDE:
                            Debug.WriteLine($"window minimized/hide");
                            DesktopWindow removed = DesktopWindowsManager.FindWindow(hwnd);
                            if (removed != null)
                            {
                                DesktopWindowsManager.mainWindowViewModel.LastChangedDesktopMonitor = removed.GetDesktopMonitor();
                                DesktopWindowsManager.RemoveWindow(removed);
                            }
                            break;
                        case User32.EventConstants.EVENT_SYSTEM_MOVESIZEEND:
                            Debug.WriteLine($"window move/size");
                            DesktopWindowsManager.mainWindowViewModel.LastChangedDesktopMonitor = new Pair<WindowsDesktop.VirtualDesktop, HMONITOR>(null, new HMONITOR());
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
                        case User32.EventConstants.EVENT_OBJECT_DRAGCOMPLETE:
                            Debug.WriteLine($"window dragged");
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

        private User32.HotKeyModifiers convertModifiers(ViewModelHotkey viewModelHotkey)
        {
            User32.HotKeyModifiers modifiers = User32.HotKeyModifiers.MOD_NONE;

            if (viewModelHotkey.Hotkey.Modifiers.HasFlag(ModifierKeys.Control))
                modifiers |= User32.HotKeyModifiers.MOD_CONTROL;
            if (viewModelHotkey.Hotkey.Modifiers.HasFlag(ModifierKeys.Shift))
                modifiers |= User32.HotKeyModifiers.MOD_SHIFT;
            if (viewModelHotkey.Hotkey.Modifiers.HasFlag(ModifierKeys.Alt))
                modifiers |= User32.HotKeyModifiers.MOD_ALT;
            if (viewModelHotkey.Hotkey.Modifiers.HasFlag(ModifierKeys.Windows))
                modifiers |= User32.HotKeyModifiers.MOD_WIN;
            
            return modifiers;
        }

        public void setKeyboardHook(HWND hWND, ObservableCollection<ViewModelHotkey> hotkeys)
        {
            foreach (var hotkey in hotkeys)
            {
                User32.RegisterHotKey(hWND, (int)hotkey.Command, convertModifiers(hotkey), (uint)KeyInterop.VirtualKeyFromKey(hotkey.Hotkey.Key));
            }
        }

        public void unsetKeyboardHook(HWND hWND, ObservableCollection<ViewModelHotkey> hotkeys)
        {
            foreach (var hotkey in hotkeys)
            {
                User32.UnregisterHotKey(hWND, (int)hotkey.Command);
            }
        }
    }
}
