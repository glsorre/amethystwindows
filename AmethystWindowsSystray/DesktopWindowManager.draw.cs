using DesktopWindowManager.Internal;
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
    partial class DesktopWindowsManager
    {
        public void Draw(Pair<VirtualDesktop, HMONITOR> key)
        {
            if (!disabled) { 
                ObservableCollection<DesktopWindow> windows = Windows[key];
                KeyValuePair<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>> desktopMonitor = new KeyValuePair<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>>(key, windows);
                float ScreenScalingFactorVert;
                int mX, mY;
                IEnumerable<Rectangle> gridGenerator;
                DrawMonitor(desktopMonitor, out ScreenScalingFactorVert, out mX, out mY, out gridGenerator);

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

                    DrawWindow1(ScreenScalingFactorVert, mX, mY, adjustedSize, w, hDWP1);
                }
                User32.EndDeferWindowPos(hDWP1.DangerousGetHandle());

                HDWP hDWP2 = User32.BeginDeferWindowPos(windows.Count);
                foreach (var w in windows.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                {
                    Rectangle adjustedSize = new Rectangle(
                        gridGenerator.ToArray()[w.Item1].X,
                        gridGenerator.ToArray()[w.Item1].Y,
                        gridGenerator.ToArray()[w.Item1].Width,
                        gridGenerator.ToArray()[w.Item1].Height
                    );

                    DrawWindow2(ScreenScalingFactorVert, mX, mY, adjustedSize, w, hDWP2);
                }
                User32.EndDeferWindowPos(hDWP2.DangerousGetHandle());

                foreach (var w in desktopMonitor.Value.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                {
                    w.Item2.GetWindowInfo();
                }
            }
        }

        public void Draw()
        {
            if (!disabled) {
                foreach (var desktopMonitor in Windows)
                {
                    float ScreenScalingFactorVert;
                    int mX, mY;
                    IEnumerable<Rectangle> gridGenerator;
                    DrawMonitor(desktopMonitor, out ScreenScalingFactorVert, out mX, out mY, out gridGenerator);

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

                        DrawWindow1(ScreenScalingFactorVert, mX, mY, adjustedSize, w, hDWP1);
                    }
                    User32.EndDeferWindowPos(hDWP1.DangerousGetHandle());

                    HDWP hDWP2 = User32.BeginDeferWindowPos(Windows.Count);
                    foreach (var w in desktopMonitor.Value.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                    {
                        Rectangle adjustedSize = new Rectangle(
                            gridGenerator.ToArray()[w.Item1].X,
                            gridGenerator.ToArray()[w.Item1].Y,
                            gridGenerator.ToArray()[w.Item1].Width,
                            gridGenerator.ToArray()[w.Item1].Height
                        );

                        DrawWindow2(ScreenScalingFactorVert, mX, mY, adjustedSize, w, hDWP2);
                    }
                    User32.EndDeferWindowPos(hDWP2.DangerousGetHandle());

                    foreach (var w in desktopMonitor.Value.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                    {
                        w.Item2.GetWindowInfo();
                    }
                }
            }
        }

        private void DrawMonitor(KeyValuePair<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>> desktopMonitor, out float ScreenScalingFactorVert, out int mX, out int mY, out IEnumerable<Rectangle> gridGenerator)
        {
            HMONITOR m = desktopMonitor.Key.Item2;
            int windowsCount = desktopMonitor.Value.Count;

            User32.MONITORINFOEX info = new User32.MONITORINFOEX();
            info.cbSize = (uint)Marshal.SizeOf(info);
            User32.GetMonitorInfo(m, ref info);

            Gdi32.SafeHDC hdc = Gdi32.CreateDC(info.szDevice);
            int LogicalScreenHeight = Gdi32.GetDeviceCaps(hdc, Gdi32.DeviceCap.VERTRES);
            int PhysicalScreenHeight = Gdi32.GetDeviceCaps(hdc, Gdi32.DeviceCap.DESKTOPVERTRES);
            int LogicalScreenWidth = Gdi32.GetDeviceCaps(hdc, Gdi32.DeviceCap.HORZRES);
            int PhysicalScreenWidth = Gdi32.GetDeviceCaps(hdc, Gdi32.DeviceCap.DESKTOPHORZRES);
            hdc.Close();

            float ScreenScalingFactorHoriz = (float)PhysicalScreenWidth / (float)LogicalScreenWidth;
            ScreenScalingFactorVert = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
            mX = info.rcWork.X + MarginLeft;
            mY = info.rcWork.Y + MarginTop;
            int mWidth = info.rcWork.Width - MarginLeft - MarginRight;
            int mHeight = info.rcWork.Height - MarginTop - MarginBottom;

            Layout mCurrentLayout;
            try
            {
                mCurrentLayout = Layouts[desktopMonitor.Key];
            }
            catch
            {
                Layouts.Add(desktopMonitor.Key, Layout.Tall);
                mCurrentLayout = Layouts[desktopMonitor.Key];
            }

            int mCurrentFactor;
            try
            {
                mCurrentFactor = Factors[desktopMonitor.Key];
            }
            catch
            {
                Factors.Add(desktopMonitor.Key, 0);
                mCurrentFactor = Factors[desktopMonitor.Key];
            }

            gridGenerator = GridGenerator(mWidth, mHeight, windowsCount, mCurrentFactor, mCurrentLayout, LayoutPadding);
        }

        private void DrawWindow1(float ScreenScalingFactorVert, int mX, int mY, Rectangle adjustedSize, Tuple<int, DesktopWindow> w, HDWP hDWP)
        {
            User32.DeferWindowPos(
                hDWP,
                w.Item2.Window,
                HWND.HWND_NOTOPMOST,
                adjustedSize.X + mX - w.Item2.Borders.left + Padding,
                adjustedSize.Y + mY - w.Item2.Borders.top + Padding,
                100,
                100,
                User32.SetWindowPosFlags.SWP_FRAMECHANGED |
                User32.SetWindowPosFlags.SWP_NOACTIVATE |
                User32.SetWindowPosFlags.SWP_NOCOPYBITS |
                User32.SetWindowPosFlags.SWP_NOZORDER |
                User32.SetWindowPosFlags.SWP_NOOWNERZORDER | 
                User32.SetWindowPosFlags.SWP_NOSIZE
                );
        }

        private void DrawWindow2(float ScreenScalingFactorVert, int mX, int mY, Rectangle adjustedSize, Tuple<int, DesktopWindow> w, HDWP hDWP)
        {
            User32.DeferWindowPos(
                hDWP,
                w.Item2.Window,
                HWND.HWND_NOTOPMOST,
                100,
                100,
                adjustedSize.Width + w.Item2.Borders.left + w.Item2.Borders.right - 2 * Padding,
                adjustedSize.Height + w.Item2.Borders.top + w.Item2.Borders.bottom - 2 * Padding,
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
