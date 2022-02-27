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
                int mX, mY;
                IEnumerable<Rectangle> gridGenerator;
                DrawMonitor(desktopMonitor, out mX, out mY, out gridGenerator);

                 foreach (var w in windows.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                {
                    User32.ShowWindow(w.Item2.Window, ShowWindowCommand.SW_RESTORE);
                }

                HDWP hDWP1 = User32.BeginDeferWindowPos(windows.Count);
                foreach (var w in windows.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                {
                    Rectangle adjustedSize = new Rectangle(
                        gridGenerator.ToArray()[w.Item1].X,
                        gridGenerator.ToArray()[w.Item1].Y,
                        gridGenerator.ToArray()[w.Item1].Width,
                        gridGenerator.ToArray()[w.Item1].Height
                    );

                    DrawWindow1(mX, mY, adjustedSize, w, hDWP1, windows.Count);
                }
                User32.EndDeferWindowPos(hDWP1.DangerousGetHandle());

                //HDWP hDWP2 = User32.BeginDeferWindowPos(windows.Count);
                //foreach (var w in windows.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                //{
                //    Rectangle adjustedSize = new Rectangle(
                //        gridGenerator.ToArray()[w.Item1].X,
                //        gridGenerator.ToArray()[w.Item1].Y,
                //        gridGenerator.ToArray()[w.Item1].Width,
                //        gridGenerator.ToArray()[w.Item1].Height
                //    );

                //    DrawWindow2(mX, mY, adjustedSize, w, hDWP2, windows.Count);
                //}
                //User32.EndDeferWindowPos(hDWP2.DangerousGetHandle());

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
                    int mX, mY;
                    IEnumerable<Rectangle> gridGenerator;
                    DrawMonitor(desktopMonitor, out mX, out mY, out gridGenerator);

                    foreach (var w in desktopMonitor.Value.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                    {
                        User32.ShowWindow(w.Item2.Window, ShowWindowCommand.SW_RESTORE);
                    }

                    HDWP hDWP1 = User32.BeginDeferWindowPos(Windows.Count);
                    foreach (var w in desktopMonitor.Value.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                    {
                        Rectangle adjustedSize = new Rectangle(
                            gridGenerator.ToArray()[w.Item1].X,
                            gridGenerator.ToArray()[w.Item1].Y,
                            gridGenerator.ToArray()[w.Item1].Width,
                            gridGenerator.ToArray()[w.Item1].Height
                        );

                        DrawWindow1(mX, mY, adjustedSize, w, hDWP1, Windows.Count);
                    }
                    User32.EndDeferWindowPos(hDWP1.DangerousGetHandle());

                    foreach (var w in desktopMonitor.Value.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                    {
                        w.Item2.GetWindowInfo();
                    }
                }
            }
        }

        private void DrawMonitor(KeyValuePair<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>> desktopMonitor, out int mX, out int mY, out IEnumerable<Rectangle> gridGenerator)
        {
            HMONITOR m = desktopMonitor.Key.Value;
            int windowsCount = desktopMonitor.Value.Count;

            User32.MONITORINFO info = new User32.MONITORINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);
            User32.GetMonitorInfo(m, ref info);

            mX = info.rcWork.X + mainWindowViewModel.MarginLeft;
            mY = info.rcWork.Y + mainWindowViewModel.MarginTop;
            int mWidth = info.rcWork.Width - mainWindowViewModel.MarginLeft - mainWindowViewModel.MarginRight;
            int mHeight = info.rcWork.Height - mainWindowViewModel.MarginTop - mainWindowViewModel.MarginBottom;

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
                    Layout.Tall
                    ));
                mCurrentLayout = mainWindowViewModel.DesktopMonitors[desktopMonitor.Key].Layout;
                mCurrentFactor = mainWindowViewModel.DesktopMonitors[desktopMonitor.Key].Factor;
            }

            gridGenerator = GridGenerator(mWidth, mHeight, windowsCount, mCurrentFactor, mCurrentLayout, mainWindowViewModel.LayoutPadding);
        }

        private void DrawWindow1(int mX, int mY, Rectangle adjustedSize, Tuple<int, DesktopWindow> w, HDWP hDWP, int windowsCount)
        {
            int X = mX + adjustedSize.X - w.Item2.BorderX + mainWindowViewModel.Padding;
            int Y = mY + adjustedSize.Y - w.Item2.BorderY / 2 + mainWindowViewModel.Padding;

            Y = Y <= mY ? mY : Y;

            User32.DeferWindowPos(
                hDWP,
                w.Item2.Window,
                HWND.HWND_NOTOPMOST,
                X,
                Y,
                adjustedSize.Width + 2 * w.Item2.BorderX - 2 * mainWindowViewModel.Padding,
                adjustedSize.Height + w.Item2.BorderY - 2 * mainWindowViewModel.Padding,
                User32.SetWindowPosFlags.SWP_NOACTIVATE |
                User32.SetWindowPosFlags.SWP_NOCOPYBITS |
                User32.SetWindowPosFlags.SWP_NOZORDER |
                User32.SetWindowPosFlags.SWP_NOOWNERZORDER
                );
        }

        private void DrawWindow2(int mX, int mY, Rectangle adjustedSize, Tuple<int, DesktopWindow> w, HDWP hDWP, int windowsCount)
        {
            User32.DeferWindowPos(
                hDWP,
                w.Item2.Window,
                HWND.HWND_NOTOPMOST,
                0,
                0,
                0,
                0,
                User32.SetWindowPosFlags.SWP_FRAMECHANGED |
                User32.SetWindowPosFlags.SWP_NOACTIVATE |
                User32.SetWindowPosFlags.SWP_NOCOPYBITS |
                User32.SetWindowPosFlags.SWP_NOZORDER |
                User32.SetWindowPosFlags.SWP_NOOWNERZORDER |
                User32.SetWindowPosFlags.SWP_NOMOVE
                );
        }
    }
}
