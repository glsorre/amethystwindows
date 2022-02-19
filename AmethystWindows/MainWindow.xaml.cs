using AmethystWindows.DesktopWindowsManager;
using DebounceThrottle;
using System;
using System.Windows;
using System.Windows.Interop;
using Vanara.PInvoke;
using WindowsDesktop;

namespace AmethystWindows
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel mainWindowViewModel = null;
        private DebounceDispatcher debounceDispatcher = new DebounceDispatcher(250);

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == (uint)User32.WindowMessage.WM_HOTKEY)
            {
                if (wParam.ToInt32() == 0x11) //space bar
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    VirtualDesktop currentDesktop = VirtualDesktop.Current;
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                    Layout currentLayout = App.DWM.RotateLayoutClockwise(currentPair);
                    string desktopLabel = string.IsNullOrEmpty(currentPair.Item1.Name) ? $"Desktop {currentPair.Item1.Id}" : currentPair.Item1.Name;
                    mainWindowViewModel.Notify(desktopLabel, currentLayout.ToString(), 100);
                    App.DWM.Draw(currentPair);
                    App.DWM.SaveLayouts();
                }
                if (wParam.ToInt32() == 0x12) //enter
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopWindow selected = App.DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    App.DWM.SetMainWindow(currentPair, selected);
                    App.DWM.Draw(currentPair);
                }
                if (wParam.ToInt32() == 0x15) // j
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopWindow selected = App.DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    App.DWM.RotateFocusedWindowClockwise(currentPair, selected);
                    App.DWM.Draw(currentPair);
                }
                if (wParam.ToInt32() == 0x14) // k
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopWindow selected = App.DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    App.DWM.RotateFocusedWindowCounterClockwise(currentPair, selected);
                    App.DWM.Draw(currentPair);
                }
                if (wParam.ToInt32() == 0x13) // l
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopWindow selected = App.DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    App.DWM.MoveWindowClockwise(currentPair, selected);
                    App.DWM.Draw(currentPair);
                }
                if (wParam.ToInt32() == 0x16) //h
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopWindow selected = App.DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    App.DWM.MoveWindowCounterClockwise(currentPair, selected);
                    App.DWM.Draw(currentPair);
                }
                if (wParam.ToInt32() == 0x17) //z
                {
                    App.DWM.ClearWindows();
                    App.DWM.CollectWindows();
                    App.DWM.Draw();
                }
                if (wParam.ToInt32() == 0x18) //p
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    VirtualDesktop currentDesktop = VirtualDesktop.Current;
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                    App.DWM.RotateMonitorClockwise(currentPair);
                }
                if (wParam.ToInt32() == 0x19) //n
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    VirtualDesktop currentDesktop = VirtualDesktop.Current;
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                    App.DWM.RotateMonitorCounterClockwise(currentPair);
                }
                if (wParam.ToInt32() == 0x21) //space bar
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    VirtualDesktop currentDesktop = VirtualDesktop.Current;
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                    Layout currentLayout = App.DWM.RotateLayoutCounterClockwise(currentPair);
                    string desktopLabel = string.IsNullOrEmpty(currentPair.Item1.Name) ? $"Desktop {currentPair.Item1.Id}" : currentPair.Item1.Name;
                    mainWindowViewModel.Notify(desktopLabel, currentLayout.ToString(), 100);
                    App.DWM.Draw(currentPair);
                    App.DWM.SaveLayouts();
                }
                if (wParam.ToInt32() == 0x22) //h
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    App.DWM.ExpandMainPane(currentPair);
                    App.DWM.SaveFactors();
                    debounceDispatcher.Debounce(() =>
                    {
                        App.DWM.Draw(currentPair);
                    });
                }
                if (wParam.ToInt32() == 0x23) //l
                {
                    debounceDispatcher.Debounce(() =>
                    {
                        HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                        Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                        App.DWM.ShrinkMainPane(currentPair);
                        App.DWM.Draw(currentPair);
                        App.DWM.SaveFactors();
                    });
                }
                if (wParam.ToInt32() == 0x24) //j
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = App.DWM.FindWindow(selectedWindow);
                    App.DWM.MoveWindowPreviousScreen(selected);
                    App.DWM.Draw();
                }
                if (wParam.ToInt32() == 0x25) //k
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = App.DWM.FindWindow(selectedWindow);
                    App.DWM.MoveWindowNextScreen(selected);
                    App.DWM.Draw();
                }
                if (wParam.ToInt32() == 0x21) //l
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    VirtualDesktop currentDesktop = VirtualDesktop.Current;
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                    App.DWM.RotateLayoutCounterClockwise(currentPair);
                    App.DWM.Draw(currentPair);
                    App.DWM.SaveLayouts();
                }
                if (wParam.ToInt32() == 0x26) //right
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = App.DWM.FindWindow(selectedWindow);
                    App.DWM.MoveWindowNextVirtualDesktop(selected);
                    debounceDispatcher.Debounce(() =>
                    {
                        App.DWM.Draw();
                    });
                }
                if (wParam.ToInt32() == 0x27) //right
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = App.DWM.FindWindow(selectedWindow);
                    App.DWM.MoveWindowPreviousVirtualDesktop(selected);
                    debounceDispatcher.Debounce(() =>
                    {
                        App.DWM.Draw();
                    });
                }
                if (wParam.ToInt32() == 0x1 || wParam.ToInt32() == 0x2 || wParam.ToInt32() == 0x3 || wParam.ToInt32() == 0x4 || wParam.ToInt32() == 0x5) //1,2,3,4,5
                {              
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = App.DWM.FindWindow(selectedWindow);
                    App.DWM.MoveWindowSpecificVirtualDesktop(selected, selected.VirtualDesktop.Id);
                    debounceDispatcher.Debounce(() =>
                    {
                        App.DWM.Draw();
                    });
                }
            }

            return IntPtr.Zero;
        }
    }
}
