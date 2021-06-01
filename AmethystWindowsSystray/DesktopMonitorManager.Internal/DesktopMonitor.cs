using DebounceThrottle;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using WindowsDesktop;

namespace DesktopMonitorManager.Internal
{
    class DesktopMonitor : ObservableCollection<DesktopWindow>
    {
        public Layout Layout { get; set; }
        public int Factor { get; set; }
        private bool Subscribed { get; set; }
        public HMONITOR Monitor { get; set; }
        public VirtualDesktop VirtualDesktop { get; set; }
        public User32.MONITORINFO MonitorInfo { get; set; }
        private DebounceDispatcher debounceDispatcher;

        private const int debounceMillis = 400;

        public DesktopMonitor(HMONITOR monitor, VirtualDesktop virtualDesktop)
        {
            Monitor = monitor;
            VirtualDesktop = virtualDesktop;

            GetMonitorInfo();

            Layout = Layout.Tall;
            Factor = 0;
            Subscribed = false;

            this.debounceDispatcher = new DebounceDispatcher(debounceMillis);
        }

        public DesktopMonitor(Layout layout, int factor, HMONITOR monitor, VirtualDesktop virtualDesktop)
        {
            Layout = layout;
            Factor = factor;
            Monitor = monitor;
            VirtualDesktop = virtualDesktop;

            GetMonitorInfo();

            Subscribed = false;
            this.debounceDispatcher = new DebounceDispatcher(debounceMillis);
        }

        private void GetMonitorInfo()
        {
            User32.MONITORINFO monitorInfo = new User32.MONITORINFO();
            monitorInfo.cbSize = (uint)Marshal.SizeOf(monitorInfo);
            User32.GetMonitorInfo(Monitor, ref monitorInfo);
            MonitorInfo = monitorInfo;
        }

        public void Save()
        {
            if (AmethystWindowsSystray.Properties.Settings.Default.DesktopMonitors != null)
            {
                int index = FindInSettings();
                if (index != -1) AmethystWindowsSystray.Properties.Settings.Default.DesktopMonitors[index] = JsonConvert.SerializeObject(this, Formatting.Indented, new DesktopMonitorConverter());
                else AmethystWindowsSystray.Properties.Settings.Default.DesktopMonitors.Add(JsonConvert.SerializeObject(this, Formatting.Indented, new DesktopMonitorConverter()));
                AmethystWindowsSystray.Properties.Settings.Default.Save();
            } else
            {
                AmethystWindowsSystray.Properties.Settings.Default.DesktopMonitors = new StringCollection();
                AmethystWindowsSystray.Properties.Settings.Default.DesktopMonitors.Add(JsonConvert.SerializeObject(this, Formatting.Indented, new DesktopMonitorConverter()));
                AmethystWindowsSystray.Properties.Settings.Default.Save();
            }
        }

        private int FindInSettings()
        {
            int result = -1;
            int index = 0;

            foreach (string desktopMonitorString in AmethystWindowsSystray.Properties.Settings.Default.DesktopMonitors)
            {
                DesktopMonitor desktopMonitor = JsonConvert.DeserializeObject<DesktopMonitor>(
                        desktopMonitorString, new DesktopMonitorConverter());

                if (this.Equals(desktopMonitor)) return index;

                index++;
            }

            return result;
        }

        public void Subscribe(bool enabled, NotifyCollectionChangedEventHandler subscriber)
        {
            if (!enabled) CollectionChanged -= subscriber;
            else if (!Subscribed) CollectionChanged += subscriber;

            Subscribed = enabled;
        }

        public void Draw(MarginPaddingStruct marginPadding)
        {
            debounceDispatcher.Debounce(() => {
                float ScreenScalingFactorVert;
                int mX, mY;
                IEnumerable<Rectangle> gridGenerator;
                GetDrawInfo(out ScreenScalingFactorVert, out mX, out mY, out gridGenerator, marginPadding);

                foreach (var w in this)
                {
                    User32.ShowWindow(w.Window, ShowWindowCommand.SW_RESTORE);
                }

                HDWP hDWP1 = User32.BeginDeferWindowPos(this.Count);
                foreach (var w in this.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                {
                    Rectangle adjustedSize = new Rectangle(
                        gridGenerator.ToArray()[w.Item1].X,
                        gridGenerator.ToArray()[w.Item1].Y,
                        gridGenerator.ToArray()[w.Item1].Width,
                        gridGenerator.ToArray()[w.Item1].Height
                    );

                    DrawWindow1(ScreenScalingFactorVert, mX, mY, adjustedSize, w.Item2, hDWP1, marginPadding.WindowPadding);
                }
                User32.EndDeferWindowPos(hDWP1.DangerousGetHandle());

                HDWP hDWP2 = User32.BeginDeferWindowPos(this.Count);
                foreach (var w in this.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                {
                    Rectangle adjustedSize = new Rectangle(
                        gridGenerator.ToArray()[w.Item1].X,
                        gridGenerator.ToArray()[w.Item1].Y,
                        gridGenerator.ToArray()[w.Item1].Width,
                        gridGenerator.ToArray()[w.Item1].Height
                    );

                    DrawWindow2(ScreenScalingFactorVert, mX, mY, adjustedSize, w.Item2, hDWP2, marginPadding.WindowPadding);
                }
                User32.EndDeferWindowPos(hDWP2.DangerousGetHandle());

                foreach (var w in this.Where((value) => value.IsRuntimePresent()))
                {
                    w.GetWindowInfo();
                }
            });
        }

        private void GetDrawInfo(out float ScreenScalingFactorVert, out int mX, out int mY, out IEnumerable<Rectangle> gridGenerator, MarginPaddingStruct marginPadding)
        {
            User32.MONITORINFOEX monitorInfoEx = new User32.MONITORINFOEX();
            monitorInfoEx.cbSize = (uint)Marshal.SizeOf(monitorInfoEx);
            User32.GetMonitorInfo(this.Monitor, ref monitorInfoEx);

            Gdi32.SafeHDC hdc = Gdi32.CreateDC(monitorInfoEx.szDevice);
            int LogicalScreenHeight = Gdi32.GetDeviceCaps(hdc, Gdi32.DeviceCap.VERTRES);
            int PhysicalScreenHeight = Gdi32.GetDeviceCaps(hdc, Gdi32.DeviceCap.DESKTOPVERTRES);
            int LogicalScreenWidth = Gdi32.GetDeviceCaps(hdc, Gdi32.DeviceCap.HORZRES);
            int PhysicalScreenWidth = Gdi32.GetDeviceCaps(hdc, Gdi32.DeviceCap.DESKTOPHORZRES);
            hdc.Close();

            float ScreenScalingFactorHoriz = (float)PhysicalScreenWidth / (float)LogicalScreenWidth;
            ScreenScalingFactorVert = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
            mX = this.MonitorInfo.rcWork.X + marginPadding.MarginLeft;
            mY = this.MonitorInfo.rcWork.Y + marginPadding.MarginTop;
            int mWidth = this.MonitorInfo.rcWork.Width - marginPadding.MarginLeft - marginPadding.MarginRight;
            int mHeight = this.MonitorInfo.rcWork.Height - marginPadding.MarginTop - marginPadding.MarginBottom;

            Layout mCurrentLayout = Layout;
            int mCurrentFactor = Factor;

            gridGenerator = GridGenerator.Generator(mWidth, mHeight, this.Count, mCurrentFactor, mCurrentLayout, marginPadding.LayoutPadding);
        }

        private void DrawWindow1(float ScreenScalingFactorVert, int mX, int mY, Rectangle adjustedSize, DesktopWindow w, HDWP hDWP, int windowPadding)
        {
            User32.DeferWindowPos(
                hDWP,
                w.Window,
                HWND.HWND_NOTOPMOST,
                adjustedSize.X + mX - w.Borders.left + windowPadding,
                adjustedSize.Y + mY - w.Borders.top + windowPadding,
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

        private void DrawWindow2(float ScreenScalingFactorVert, int mX, int mY, Rectangle adjustedSize, DesktopWindow w, HDWP hDWP, int windowPadding)
        {
            User32.DeferWindowPos(
                hDWP,
                w.Window,
                HWND.HWND_NOTOPMOST,
                100,
                100,
                adjustedSize.Width + w.Borders.left + w.Borders.right - 2 * windowPadding,
                adjustedSize.Height + w.Borders.top + w.Borders.bottom - 2 * windowPadding,
                User32.SetWindowPosFlags.SWP_FRAMECHANGED |
                User32.SetWindowPosFlags.SWP_NOACTIVATE |
                User32.SetWindowPosFlags.SWP_NOCOPYBITS |
                User32.SetWindowPosFlags.SWP_NOZORDER |
                User32.SetWindowPosFlags.SWP_NOOWNERZORDER |
                User32.SetWindowPosFlags.SWP_NOMOVE
                );
        }

        public override bool Equals(object obj)
        {
            return obj is DesktopMonitor monitor &&
                   EqualityComparer<HMONITOR>.Default.Equals(this.Monitor, monitor.Monitor) &&
                   EqualityComparer<VirtualDesktop>.Default.Equals(VirtualDesktop, monitor.VirtualDesktop);
        }

        public override int GetHashCode()
        {
            int hashCode = -509871200;
            hashCode = hashCode * -1521134295 + Monitor.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<VirtualDesktop>.Default.GetHashCode(VirtualDesktop);
            return hashCode;
        }
    }
}
