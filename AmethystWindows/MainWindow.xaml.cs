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
        private MainWindowViewModel mainWindowViewModel;
        private DebounceDispatcher debounceDispatcher = new DebounceDispatcher(250);

        public MainWindow()
        {
            InitializeComponent();
            mainWindowViewModel = DataContext as MainWindowViewModel;
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
                if (wParam.ToInt32() == 0x11)
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    VirtualDesktop currentDesktop = VirtualDesktop.Current;
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                    ViewModelDesktopMonitor viewModelDesktopMonitor = mainWindowViewModel.DesktopMonitors[currentPair];
                    viewModelDesktopMonitor.RotateLayoutClockwise();
                    //string desktopLabel = string.IsNullOrEmpty(currentPair.Key.Name) ? $"Desktop {currentPair.Key.Id}" : currentPair.Key.Name;
                }
                if (wParam.ToInt32() == 0x12)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopWindow selected = App.DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    App.DWM.SetMainWindow(currentPair, selected);
                }
                if (wParam.ToInt32() == 0x15)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopWindow selected = App.DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    App.DWM.RotateFocusedWindowClockwise(currentPair, selected);
                }
                if (wParam.ToInt32() == 0x14)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopWindow selected = App.DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    App.DWM.RotateFocusedWindowCounterClockwise(currentPair, selected);
                }
                if (wParam.ToInt32() == 0x13)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopWindow selected = App.DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    App.DWM.MoveWindowClockwise(currentPair, selected);
                }
                if (wParam.ToInt32() == 0x16)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopWindow selected = App.DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    App.DWM.MoveWindowCounterClockwise(currentPair, selected);
                }
                if (wParam.ToInt32() == 0x17)
                {
                    mainWindowViewModel.RedrawCommand.Execute(null);
                }
                if (wParam.ToInt32() == 0x18)
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    VirtualDesktop currentDesktop = VirtualDesktop.Current;
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                    App.DWM.RotateMonitorClockwise(currentPair);
                }
                if (wParam.ToInt32() == 0x19)
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    VirtualDesktop currentDesktop = VirtualDesktop.Current;
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                    App.DWM.RotateMonitorCounterClockwise(currentPair);
                }
                if (wParam.ToInt32() == 0x21)
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    VirtualDesktop currentDesktop = VirtualDesktop.Current;
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                    ViewModelDesktopMonitor viewModelDesktopMonitor = mainWindowViewModel.DesktopMonitors[currentPair];
                    viewModelDesktopMonitor.RotateLayoutCounterClockwise();
                    //string desktopLabel = string.IsNullOrEmpty(currentPair.Key.Name) ? $"Desktop {currentPair.Key.Id}" : currentPair.Key.Name;
                    //mainWindowViewModel.Notify(desktopLabel, viewModelDesktopMonitor.Layout.ToString(), 100);

                }
                if (wParam.ToInt32() == 0x22)
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    ViewModelDesktopMonitor viewModelDesktopMonitor = mainWindowViewModel.DesktopMonitors[currentPair];
                    viewModelDesktopMonitor.Expand();
                }
                if (wParam.ToInt32() == 0x23)
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    ViewModelDesktopMonitor viewModelDesktopMonitor = mainWindowViewModel.DesktopMonitors[currentPair];
                    viewModelDesktopMonitor.Shrink();
                }
                if (wParam.ToInt32() == 0x24)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = App.DWM.FindWindow(selectedWindow);
                    App.DWM.MoveWindowPreviousScreen(selected);
                }
                if (wParam.ToInt32() == 0x25)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = App.DWM.FindWindow(selectedWindow);
                    App.DWM.MoveWindowNextScreen(selected);
                }
                if (wParam.ToInt32() == 0x21)
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    VirtualDesktop currentDesktop = VirtualDesktop.Current;
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                    ViewModelDesktopMonitor viewModelDesktopMonitor = mainWindowViewModel.DesktopMonitors[currentPair];
                    viewModelDesktopMonitor.RotateLayoutCounterClockwise();
                }
                if (wParam.ToInt32() == 0x26)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = App.DWM.FindWindow(selectedWindow);
                    App.DWM.MoveWindowNextVirtualDesktop(selected);

                }
                if (wParam.ToInt32() == 0x27)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = App.DWM.FindWindow(selectedWindow);
                    App.DWM.MoveWindowPreviousVirtualDesktop(selected);

                }
                if (wParam.ToInt32() == 0x1 || wParam.ToInt32() == 0x2 || wParam.ToInt32() == 0x3 || wParam.ToInt32() == 0x4 || wParam.ToInt32() == 0x5) //1,2,3,4,5
                {              
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = App.DWM.FindWindow(selectedWindow);
                    App.DWM.MoveWindowSpecificVirtualDesktop(selected, selected.VirtualDesktop.Id);
                }
            }

            return IntPtr.Zero;
        }
    }
}
