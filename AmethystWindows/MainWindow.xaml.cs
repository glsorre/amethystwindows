using AmethystWindows.DesktopWindowsManager;
using AmethystWindows.Hotkeys;
using DebounceThrottle;
using System;
using System.Windows;
using System.Windows.Interop;
using Vanara.PInvoke;
using Windows.UI.Core;
using WindowsDesktop;

namespace AmethystWindows
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel mainWindowViewModel;

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
            if (User32.GetActiveWindow() == new WindowInteropHelper(this).Handle)
            {
                //handled = true;
                return IntPtr.Zero;
            }

            if (msg == (uint)User32.WindowMessage.WM_HOTKEY)
            {
                if (wParam.ToInt32() == (Int32)HotkeyCommand.rotateLayoutClockwise)
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    VirtualDesktop currentDesktop = VirtualDesktop.Current;
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                    ViewModelDesktopMonitor viewModelDesktopMonitor = mainWindowViewModel.DesktopMonitors[currentPair];
                    viewModelDesktopMonitor.RotateLayoutClockwise();
                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.rotateLayoutCounterclockwise)
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    VirtualDesktop currentDesktop = VirtualDesktop.Current;
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                    ViewModelDesktopMonitor viewModelDesktopMonitor = mainWindowViewModel.DesktopMonitors[currentPair];
                    viewModelDesktopMonitor.RotateLayoutClockwise();
                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.setMainPane)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopWindow selected = App.DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    App.DWM.SetMainWindow(currentPair, selected);
                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.setSecondaryPane)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopWindow selected = App.DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    App.DWM.SetSecondaryWindow(currentPair, selected);
                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.swapFocusedClockwise)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopWindow selected = App.DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    App.DWM.RotateFocusedWindowClockwise(currentPair, selected);
                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.swapFocusedCounterclockwise)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopWindow selected = App.DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    App.DWM.RotateFocusedWindowCounterClockwise(currentPair, selected);
                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.swapFocusedClockwise)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopWindow selected = App.DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    App.DWM.MoveWindowClockwise(currentPair, selected);
                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.swapFocusedCounterclockwise)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopWindow selected = App.DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    App.DWM.MoveWindowCounterClockwise(currentPair, selected);
                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.redrawSimple)
                {
                    App.DWM.Draw();
                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.redrawForced)
                {
                    mainWindowViewModel.RedrawCommand.Execute(null);
                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.moveFocusNextScreen)
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    VirtualDesktop currentDesktop = VirtualDesktop.Current;
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                    App.DWM.RotateMonitorClockwise(currentPair);
                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.moveFocusPreviousScreen)
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    VirtualDesktop currentDesktop = VirtualDesktop.Current;
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                    App.DWM.RotateMonitorCounterClockwise(currentPair);
                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.expandMainPane)
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    ViewModelDesktopMonitor viewModelDesktopMonitor = mainWindowViewModel.DesktopMonitors[currentPair];
                    viewModelDesktopMonitor.Expand();
                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.shrinkMainPane)
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    ViewModelDesktopMonitor viewModelDesktopMonitor = mainWindowViewModel.DesktopMonitors[currentPair];
                    viewModelDesktopMonitor.Shrink();
                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.moveFocusedPreviousScreen)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = App.DWM.FindWindow(selectedWindow);
                    App.DWM.MoveWindowPreviousScreen(selected);
                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.moveFocusedNextScreen)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = App.DWM.FindWindow(selectedWindow);
                    App.DWM.MoveWindowNextScreen(selected);
                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.moveFocusedNextSpace)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = App.DWM.FindWindow(selectedWindow);
                    App.DWM.MoveWindowNextVirtualDesktop(selected);

                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.moveFocusedPreviousSpace)
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = App.DWM.FindWindow(selectedWindow);
                    App.DWM.MoveWindowPreviousVirtualDesktop(selected);

                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.moveFocusedToSpace1 || wParam.ToInt32() == (Int32)HotkeyCommand.moveFocusedToSpace2 || wParam.ToInt32() == (Int32)HotkeyCommand.moveFocusedToSpace3 || wParam.ToInt32() == (Int32)HotkeyCommand.moveFocusedToSpace4 || wParam.ToInt32() == (Int32)HotkeyCommand.moveFocusedToSpace5) //1,2,3,4,5
                {              
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = App.DWM.FindWindow(selectedWindow);
                    App.DWM.MoveWindowSpecificVirtualDesktop(selected, selected.VirtualDesktop.Id);
                }
                if (wParam.ToInt32() == (Int32)HotkeyCommand.switchToSpace1 || wParam.ToInt32() == (Int32)HotkeyCommand.switchToSpace2 || wParam.ToInt32() == (Int32)HotkeyCommand.switchToSpace3 || wParam.ToInt32() == (Int32)HotkeyCommand.switchToSpace4 || wParam.ToInt32() == (Int32)HotkeyCommand.switchToSpace5) //1,2,3,4,5
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = App.DWM.FindWindow(selectedWindow);
                    App.DWM.MoveToSpecificVirtualDesktop(selected, selected.VirtualDesktop.Id);
                }
            }

            // handled = true;
            return IntPtr.Zero;
        }
    }
}
