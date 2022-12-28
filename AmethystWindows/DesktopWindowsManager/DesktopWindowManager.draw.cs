using AmethystWindows.GridGenerator;
using AmethystWindows.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Vanara.PInvoke;
using WindowsDesktop;

namespace AmethystWindows.DesktopWindowsManager
{
    partial class DesktopWindowsManager
    {
        public void Draw(Pair<VirtualDesktop, HMONITOR> key)
        {
            if (!mainWindowViewModel.Disabled) { 
                ObservableCollection<DesktopWindow> windows = Windows[key];
                KeyValuePair<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>> desktopMonitor = new KeyValuePair<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>>(key, windows);
                int mX, mY, mWidth, mHeight;
                Rectangle[] grid;
                DrawMonitor(desktopMonitor, out mX, out mY, out mWidth, out mHeight, out grid);

                foreach (var w in windows.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                {
                    User32.ShowWindow(w.Item2.Window, ShowWindowCommand.SW_RESTORE);
                }

                HDWP hDWP1 = User32.BeginDeferWindowPos(windows.Count);
                foreach (var w in windows.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                {
                    Rectangle adjustedSize = new Rectangle(
                        grid[w.Item1].X,
                        grid[w.Item1].Y,
                        grid[w.Item1].Width,
                        grid[w.Item1].Height
                    );

                    DrawWindow(mX, mY, mWidth, mHeight, adjustedSize, w, hDWP1, windows.Count);
                }
                User32.EndDeferWindowPos(hDWP1.DangerousGetHandle());

                if (windows.Count == 1) User32.ShowWindow(windows[0].Window, ShowWindowCommand.SW_MAXIMIZE);

                foreach (var w in desktopMonitor.Value.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                {
                    w.Item2.GetWindowInfo();
                }
            }
        }

        public void Draw()
        {
            if (!mainWindowViewModel.Disabled) {
                foreach (var desktopMonitor in Windows)
                {
                    int mX, mY, mWidth, mHeight;
                    Rectangle[] grid;
                    DrawMonitor(desktopMonitor, out mX, out mY, out mWidth, out mHeight, out grid);

                    foreach (var w in desktopMonitor.Value.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                    {
                        User32.ShowWindow(w.Item2.Window, ShowWindowCommand.SW_RESTORE);
                    }

                    HDWP hDWP1 = User32.BeginDeferWindowPos(Windows.Count);
                    foreach (var w in desktopMonitor.Value.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                    {
                        Rectangle adjustedSize = new Rectangle(
                            grid[w.Item1].X,
                            grid[w.Item1].Y,
                            grid[w.Item1].Width,
                            grid[w.Item1].Height
                        );

                        DrawWindow(mX, mY, mWidth, mHeight, adjustedSize, w, hDWP1, Windows.Count);
                    }
                    User32.EndDeferWindowPos(hDWP1.DangerousGetHandle());

                    if (desktopMonitor.Value.Count == 1) User32.ShowWindow(desktopMonitor.Value[0].Window, ShowWindowCommand.SW_MAXIMIZE);

                    foreach (var w in desktopMonitor.Value.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                    {
                        w.Item2.GetWindowInfo();
                    }
                }
            }
        }

        private void DrawMonitor(KeyValuePair<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>> desktopMonitor, out int mX, out int mY, out int mWidth, out int mHeight, out Rectangle[] grid)
        {
            HMONITOR m = desktopMonitor.Key.Value;
            int windowsCount = desktopMonitor.Value.Count;

            User32.MONITORINFO info = new User32.MONITORINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);
            User32.GetMonitorInfo(m, ref info);

            mX = info.rcWork.X + mainWindowViewModel.MarginLeft;
            mY = info.rcWork.Y + mainWindowViewModel.MarginTop;
            mWidth = info.rcWork.Width - mainWindowViewModel.MarginLeft - mainWindowViewModel.MarginRight;
            mHeight = info.rcWork.Height - mainWindowViewModel.MarginTop - mainWindowViewModel.MarginBottom;

            Layout mCurrentLayout;
            int mCurrentFactor;
            try
            {
                mCurrentLayout = mainWindowViewModel.DesktopMonitors[desktopMonitor.Key].Layout;
                mCurrentFactor = mainWindowViewModel.DesktopMonitors[desktopMonitor.Key].Factor;
            }
            catch
            {
                mainWindowViewModel.DesktopMonitors.Add(new ViewModelDesktopMonitor(
                    desktopMonitor.Key.Value,
                    desktopMonitor.Key.Key,
                    0,
                    Layout.TallLeft
                    ));
                mCurrentLayout = mainWindowViewModel.DesktopMonitors[desktopMonitor.Key].Layout;
                mCurrentFactor = mainWindowViewModel.DesktopMonitors[desktopMonitor.Key].Factor;
            }

            System.Diagnostics.Debug.WriteLine("Layout: " + mCurrentLayout.ToString());
            System.Diagnostics.Debug.WriteLine("Count: " + windowsCount.ToString());

            grid = GridGenerator.Generate(mCurrentLayout, mWidth, mHeight, windowsCount, mainWindowViewModel.LayoutPadding, mCurrentFactor, mainWindowViewModel.Step);
        }

        private void DrawWindow(int mX, int mY, int mWidth, int mHeight, Rectangle adjustedSize, Tuple<int, DesktopWindow> w, HDWP hDWP, int windowsCount)
        {
            GridRectangle gridRectangle = new GridRectangle(
                new Rectangle(adjustedSize.X, adjustedSize.Y, adjustedSize.Width, adjustedSize.Height),
                new Point(mX, mY),
                new Point(w.Item2.BorderX, w.Item2.BorderY),
                w.Item2.OffsetTL,
                w.Item2.OffsetBR,
                new Point(mainWindowViewModel.Padding, mainWindowViewModel.Padding)
            );

            Rectangle windowRectangle = gridRectangle.Position();

            User32.DeferWindowPos(
                hDWP,
                w.Item2.Window,
                HWND.HWND_NOTOPMOST,
                windowRectangle.X,
                windowRectangle.Y,
                windowRectangle.Width,
                windowRectangle.Height,
                User32.SetWindowPosFlags.SWP_NOACTIVATE |
                User32.SetWindowPosFlags.SWP_NOCOPYBITS |
                User32.SetWindowPosFlags.SWP_NOZORDER |
                User32.SetWindowPosFlags.SWP_NOOWNERZORDER
                );
        }
    }
}
