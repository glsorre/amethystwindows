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
            ObservableCollection<DesktopWindow> windows = Windows[key];
            KeyValuePair<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>> desktopMonitor = new KeyValuePair<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>>(key, windows);
            float ScreenScalingFactorVert;
            int mX, mY;
            IEnumerable<Tuple<int, int, int, int>> gridGenerator;
            DrawMonitor(desktopMonitor, out ScreenScalingFactorVert, out mX, out mY, out gridGenerator);

            HDWP hDWP = User32.BeginDeferWindowPos(windows.Count);
            foreach (var w in desktopMonitor.Value.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
            {
                DrawWindow(ScreenScalingFactorVert, mX, mY, gridGenerator, w, hDWP);
            }
            User32.EndDeferWindowPos(hDWP.DangerousGetHandle());

            foreach (var w in desktopMonitor.Value.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
            {
                w.Item2.GetWindowInfo();
            }
        }

        public void Draw()
        {
            foreach (var desktopMonitor in Windows)
            {
                float ScreenScalingFactorVert;
                int mX, mY;
                IEnumerable<Tuple<int, int, int, int>> gridGenerator;
                DrawMonitor(desktopMonitor, out ScreenScalingFactorVert, out mX, out mY, out gridGenerator);

                HDWP hDWP = User32.BeginDeferWindowPos(Windows.Count);
                foreach (var w in desktopMonitor.Value.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                {
                    DrawWindow(ScreenScalingFactorVert, mX, mY, gridGenerator, w, hDWP);
                }
                User32.EndDeferWindowPos(hDWP.DangerousGetHandle());

                foreach (var w in desktopMonitor.Value.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                {
                    w.Item2.GetWindowInfo();
                }
            }
        }

        private void DrawMonitor(KeyValuePair<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>> desktopMonitor, out float ScreenScalingFactorVert, out int mX, out int mY, out IEnumerable<Tuple<int, int, int, int>> gridGenerator)
        {
            HMONITOR m = desktopMonitor.Key.Item2;
            int windowsCount = desktopMonitor.Value.Count();

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
            mX = info.rcMonitor.X;
            mY = info.rcMonitor.Y;
            int mWidth = info.rcWork.Width;
            int mHeight = info.rcWork.Height;

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

            if (!Factors.ContainsKey(desktopMonitor.Key))
            {
                Factors[desktopMonitor.Key] = 0;
            }

            gridGenerator = GridGenerator(mWidth, mHeight, windowsCount, Factors[desktopMonitor.Key], mCurrentLayout);
        }

        private void DrawWindow(float ScreenScalingFactorVert, int mX, int mY, IEnumerable<Tuple<int, int, int, int>> gridGenerator, Tuple<int, DesktopWindow> w, HDWP hDWP)
        {
            RECT adjustedSize = new RECT(new Rectangle(
                gridGenerator.ToArray()[w.Item1].Item1,
                gridGenerator.ToArray()[w.Item1].Item2,
                gridGenerator.ToArray()[w.Item1].Item3,
                gridGenerator.ToArray()[w.Item1].Item4
                ));

            //Prepare the WINDOWPLACEMENT structure.
            User32.WINDOWPLACEMENT placement = new User32.WINDOWPLACEMENT();
            placement.length = (uint)Marshal.SizeOf(placement);

            //Get the window's current placement.
            User32.GetWindowPlacement(w.Item2.Window, ref placement);
            placement.showCmd = ShowWindowCommand.SW_RESTORE;

            //Perform the action.
            User32.SetWindowPlacement(w.Item2.Window, ref placement);

            User32.DeferWindowPos(
                hDWP,
                w.Item2.Window,
                HWND.HWND_NOTOPMOST,
                adjustedSize.X + mX - w.Item2.Borders.left + Padding,
                adjustedSize.Y + mY - w.Item2.Borders.top + Padding,
                adjustedSize.Width + w.Item2.Borders.left + w.Item2.Borders.right - 2 * Padding,
                adjustedSize.Height + w.Item2.Borders.top + w.Item2.Borders.bottom - 2 * Padding,
                User32.SetWindowPosFlags.SWP_FRAMECHANGED | 
                User32.SetWindowPosFlags.SWP_NOACTIVATE |
                User32.SetWindowPosFlags.SWP_NOCOPYBITS |
                User32.SetWindowPosFlags.SWP_NOZORDER |
                User32.SetWindowPosFlags.SWP_NOOWNERZORDER
                );
        }
    }
}
