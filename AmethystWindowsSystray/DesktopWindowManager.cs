using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using WindowsDesktop;

namespace AmethystWindowsSystray
{
    public enum Layout : ushort
    {
        Horizontal = 0,
        Vertical = 1,
        HorizGrid = 2,
        VertGrid = 3,
        Monocle = 4,
        Wide = 5,
        Tall = 6
    }

    public struct Pair<K, V>
    {
        public K Item1 { get; set; }
        public V Item2 { get; set; }

        public Pair(K item1, V item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override bool Equals(object obj)
        {
            return obj is Pair<K, V> pair &&
                   EqualityComparer<K>.Default.Equals(Item1, pair.Item1) &&
                   EqualityComparer<V>.Default.Equals(Item2, pair.Item2);
        }

        public override int GetHashCode()
        {
            int hashCode = -1030903623;
            hashCode = hashCode * -1521134295 + EqualityComparer<K>.Default.GetHashCode(Item1);
            hashCode = hashCode * -1521134295 + EqualityComparer<V>.Default.GetHashCode(Item2);
            return hashCode;
        }
    }

    class DesktopWindowsManager
    {
        public Dictionary<Pair<VirtualDesktop, HMONITOR>, Layout> Layouts;
        public Dictionary<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>> Windows;
        public Dictionary<Pair<VirtualDesktop, HMONITOR>, bool> WindowsSubcribed;
        public int Padding;

        public DesktopWindowsManager()
        {
            this.Padding = 5;
            this.Layouts = new Dictionary<Pair<VirtualDesktop, HMONITOR>, Layout>();
            this.Windows = new Dictionary<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>>();
            this.WindowsSubcribed = new Dictionary<Pair<VirtualDesktop, HMONITOR>, bool>();
        }

        public void AddWindow(DesktopWindow desktopWindow)
        {
            Windows[new Pair<VirtualDesktop, HMONITOR>(desktopWindow.VirtualDesktop, desktopWindow.MonitorHandle)].Add(desktopWindow);
        }

        public void RemoveWindow(DesktopWindow desktopWindow)
        {
            Windows[new Pair<VirtualDesktop, HMONITOR>(desktopWindow.VirtualDesktop, desktopWindow.MonitorHandle)].Remove(desktopWindow);
        }

        public void RepositionWindow(DesktopWindow oldDesktopWindow, DesktopWindow newDesktopWindow)
        {
            RemoveWindow(oldDesktopWindow);
            AddWindow(newDesktopWindow);
        }

        public DesktopWindow FindWindow(HWND hWND)
        {
            List<DesktopWindow> desktopWindows = new List<DesktopWindow>();
            foreach (var desktopMonitor in Windows)
            {
                desktopWindows.AddRange(Windows[new Pair<VirtualDesktop, HMONITOR>(desktopMonitor.Key.Item1, desktopMonitor.Key.Item2)].Where(window => window.Window == hWND));
            }
            return desktopWindows.FirstOrDefault();
        }

        public DesktopWindow GetWindowByHandlers(HWND hWND, HMONITOR hMONITOR, VirtualDesktop desktop)
        {
            return Windows[new Pair<VirtualDesktop, HMONITOR>(desktop, hMONITOR)].FirstOrDefault(window => window.Window == hWND);
        }

        private IEnumerable<Tuple<int, int, int, int>> GridGenerator(int mWidth, int mHeight, int windowsCount, Layout layout)
        {
            if (layout == Layout.Horizontal)
            {
                int horizSize = mWidth / windowsCount;
                int j = 0;
                for (int i = 0; i < windowsCount; i++)
                {
                    yield return new Tuple<int, int, int, int>(i * horizSize, j, horizSize, mHeight);
                }
            }
            else if (layout == Layout.Vertical)
            {
                int vertSize = mHeight / windowsCount;
                int j = 0;
                for (int i = 0; i < windowsCount; i++)
                {
                    yield return new Tuple<int, int, int, int>(j, i * vertSize, mWidth, vertSize);
                }
            }
            else if (layout == Layout.HorizGrid)
            {
                int i = 0;
                int j = 0;
                int horizStep = Math.Max((int)Math.Sqrt(windowsCount), 1);
                int vertStep = Math.Max(windowsCount / horizStep, 1);
                int tiles = horizStep * vertStep;
                int horizSize = mWidth / horizStep;
                int vertSize = mHeight / vertStep;
                bool isFirstLine = true;

                if (windowsCount != tiles || windowsCount == 3)
                {
                    if (windowsCount == 3)
                    {
                        vertStep--;
                        vertSize = mHeight / vertStep;
                    }

                    while (windowsCount > 0)
                    {
                        yield return new Tuple<int, int, int, int>(i * horizSize, j * vertSize, horizSize, vertSize);
                        i++;
                        if (i >= horizStep)
                        {
                            i = 0;
                            j++;
                        }
                        if (j == vertStep - 1 && isFirstLine)
                        {
                            horizStep++;
                            horizSize = mWidth / horizStep;
                            isFirstLine = false;
                        }
                        windowsCount--;
                    }
                }
                else
                {
                    while (windowsCount > 0)
                    {
                        yield return new Tuple<int, int, int, int>(i * horizSize, j * vertSize, horizSize, vertSize);
                        i++;
                        if (i >= horizStep)
                        {
                            i = 0;
                            j++;
                        }
                        windowsCount--;
                    }
                }
            }
            else if (layout == Layout.VertGrid)
            {
                int i = 0;
                int j = 0;
                int vertStep = Math.Max((int)Math.Sqrt(windowsCount), 1);
                int horizStep = Math.Max(windowsCount / vertStep, 1);
                int tiles = horizStep * vertStep;
                int vertSize = mHeight / vertStep;
                int horizSize = mWidth / horizStep;
                bool isFirstLine = true;

                if (windowsCount != tiles || windowsCount == 3)
                {
                    if (windowsCount == 3)
                    {
                        horizStep--;
                        horizSize = mWidth / horizStep;
                    }

                    while (windowsCount > 0)
                    {
                        yield return new Tuple<int, int, int, int>(i * horizSize, j * vertSize, horizSize, vertSize);
                        j++;
                        if (j >= vertStep)
                        {
                            j = 0;
                            i++;
                        }
                        if (i == horizStep - 1 && isFirstLine)
                        {
                            vertStep++;
                            vertSize = mHeight / vertStep;
                            isFirstLine = false;
                        }
                        windowsCount--;
                    }
                }
                else
                {
                    while (windowsCount > 0)
                    {
                        yield return new Tuple<int, int, int, int>(i * horizSize, j * vertSize, horizSize, vertSize);
                        j++;
                        if (j >= vertStep)
                        {
                            j = 0;
                            i++;
                        }
                        windowsCount--;
                    }
                }
            }
            else if (layout == Layout.Monocle)
            {
                for (int i = 0; i < windowsCount; i++)
                {
                    yield return new Tuple<int, int, int, int>(0, 0, mWidth, mHeight);
                }
            }
            else if (layout == Layout.Wide)
            {
                if (windowsCount == 1) yield return new Tuple<int, int, int, int>(0, 0, mWidth, mHeight);
                else
                {
                    int size = mWidth / (windowsCount - 1);
                    for (int i = 0; i < windowsCount - 1; i++)
                    {
                        if (i == 0) yield return new Tuple<int, int, int, int>(0, 0, mWidth, mHeight / 2);
                        yield return new Tuple<int, int, int, int>(i * size, mHeight / 2, size, mHeight / 2);
                    }
                } 
            }
            else if (layout == Layout.Tall)
            {
                if (windowsCount == 1) yield return new Tuple<int, int, int, int>(0, 0, mWidth, mHeight);
                else
                {
                    int size = mHeight / (windowsCount - 1);
                    for (int i = 0; i < windowsCount - 1; i++)
                    {
                        if (i == 0) yield return new Tuple<int, int, int, int>(0, 0, mWidth / 2, mHeight);
                        yield return new Tuple<int, int, int, int>(mWidth / 2, i * size, mWidth / 2, size);
                    }
                }
            }
        }

        public Layout RotateLayout(Layout currentLayout)
        {
            IEnumerable<Layout> values = Enum.GetValues(typeof(Layout)).Cast<Layout>();
            if (currentLayout == values.Max())
            {
                return Layout.Horizontal;
            }
            else
            {
                return ++currentLayout;
            }
        }

        public void Draw(DesktopWindow dekstopWindow)
        {
            Pair<VirtualDesktop, HMONITOR> key = new Pair<VirtualDesktop, HMONITOR>(dekstopWindow.VirtualDesktop, dekstopWindow.MonitorHandle);
            ObservableCollection<DesktopWindow> windows = Windows[key];
            KeyValuePair<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>> desktopMonitor = new KeyValuePair<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>>(key, windows);
            float ScreenScalingFactorVert;
            int mX, mY;
            IEnumerable<Tuple<int, int, int, int>> gridGenerator;
            DrawMonitor(desktopMonitor, out ScreenScalingFactorVert, out mX, out mY, out gridGenerator);

            foreach (var w in desktopMonitor.Value.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
            {
                DrawWindow(ScreenScalingFactorVert, mX, mY, gridGenerator, w);
            }
        }

        public void Draw(Pair<VirtualDesktop, HMONITOR> key)
        {
            ObservableCollection<DesktopWindow> windows = Windows[key];
            KeyValuePair<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>> desktopMonitor = new KeyValuePair<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>>(key, windows);
            float ScreenScalingFactorVert;
            int mX, mY;
            IEnumerable<Tuple<int, int, int, int>> gridGenerator;
            DrawMonitor(desktopMonitor, out ScreenScalingFactorVert, out mX, out mY, out gridGenerator);

            foreach (var w in desktopMonitor.Value.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
            {
                DrawWindow(ScreenScalingFactorVert, mX, mY, gridGenerator, w);
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

                foreach (var w in desktopMonitor.Value.Select((value, i) => new Tuple<int, DesktopWindow>(i, value)))
                {
                    DrawWindow(ScreenScalingFactorVert, mX, mY, gridGenerator, w);
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
                Layouts.Add(desktopMonitor.Key, Layout.Horizontal);
                mCurrentLayout = Layouts[desktopMonitor.Key];
            }

            gridGenerator = GridGenerator(mWidth, mHeight, windowsCount, mCurrentLayout);
        }

        private void DrawWindow(float ScreenScalingFactorVert, int mX, int mY, IEnumerable<Tuple<int, int, int, int>> gridGenerator, Tuple<int, DesktopWindow> w)
        {
            //Prepare the WINDOWPLACEMENT structure.
            User32.WINDOWPLACEMENT placement = new User32.WINDOWPLACEMENT();
            placement.length = (uint)Marshal.SizeOf(placement);

            //Get the window's current placement.
            User32.GetWindowPlacement(w.Item2.Window, ref placement);
            placement.showCmd = ShowWindowCommand.SW_RESTORE;

            //Perform the action.
            User32.SetWindowPlacement(w.Item2.Window, ref placement);

            RECT adjustedSize = new RECT(new Rectangle(
                gridGenerator.ToArray()[w.Item1].Item1,
                gridGenerator.ToArray()[w.Item1].Item2,
                gridGenerator.ToArray()[w.Item1].Item3,
                gridGenerator.ToArray()[w.Item1].Item4
                ));

            User32.AdjustWindowRectExForDpi(
                ref adjustedSize,
                w.Item2.Info.dwStyle,
                false, w.Item2.Info.dwExStyle,
                (uint)(ScreenScalingFactorVert / 96)
                );

            User32.SetWindowPos(
                w.Item2.Window,
                HWND.HWND_NOTOPMOST,
                adjustedSize.X + mX - w.Item2.Borders.left + Padding,
                adjustedSize.Y + mY - w.Item2.Borders.top + Padding,
                adjustedSize.Width + w.Item2.Borders.left + w.Item2.Borders.right - 2 * Padding,
                adjustedSize.Height + w.Item2.Borders.top + w.Item2.Borders.bottom - 2 * Padding,
                User32.SetWindowPosFlags.SWP_NOACTIVATE
                );

            User32.SetWindowPos(
                w.Item2.Window,
                HWND.HWND_NOTOPMOST,
                adjustedSize.X + mX - w.Item2.Borders.left + Padding,
                adjustedSize.Y + mY - w.Item2.Borders.top + Padding,
                adjustedSize.Width + w.Item2.Borders.left + w.Item2.Borders.right - 2 * Padding,
                adjustedSize.Height + w.Item2.Borders.top + w.Item2.Borders.bottom - 2 * Padding,
                User32.SetWindowPosFlags.SWP_NOMOVE
                );

            w.Item2.GetInfo();
        }
    }

    class DesktopWindow
    {
        public VirtualDesktop VirtualDesktop { get; set; }
        public User32.MONITORINFO Monitor { get; set; }
        public HMONITOR MonitorHandle { get; set; }
        public HWND Window { get; set; }
        public User32.WINDOWINFO Info { get; set; }
        public String AppName { get; set; }
        public RECT Borders;

        private static readonly string[] WindowsClassNamesToSkip =
        {
            "Shell_TrayWnd",
            "DV2ControlHost",
            "MsgrIMEWindowClass",
            "SysShadow",
            "Button"
        };

        public DesktopWindow(HWND window)
        {
            Window = window;
            GetWindowInfo();
        }

        public bool isPresent()
        {
            return User32.IsWindowVisible(Window) &&
                !User32.IsIconic(Window) &&
                !IsBackgroundAppWindow() &&
                IsAltTabWindow();
        }

        public bool isRuntimeValuable()
        {
            return IsAltTabWindow();
        }

        private bool IsBackgroundAppWindow()
        {
            DwmApi.DWM_CLOAKED CloakedVal = new DwmApi.DWM_CLOAKED();
            HRESULT hRes = DwmApi.DwmGetWindowAttribute(Window, DwmApi.DWMWINDOWATTRIBUTE.DWMWA_CLOAKED, out CloakedVal);
            if (hRes != HRESULT.S_OK)
            {
                CloakedVal = 0;
            }
            return CloakedVal != 0 ? true : false;
        }

        private bool IsAltTabWindow()
        {
            if (Info.dwExStyle.HasFlag(User32.WindowStylesEx.WS_EX_TOOLWINDOW)) return false;

            var classNameStringBuilder = new StringBuilder(256);
            var length = User32.GetClassName(Window, classNameStringBuilder, classNameStringBuilder.Capacity);
            if (length == 0)
                return false;

            var className = classNameStringBuilder.ToString();

            if (Array.IndexOf(WindowsClassNamesToSkip, className) > -1) return false;

            HWND hwndWalk = User32.GetAncestor(Window, User32.GetAncestorFlag.GA_ROOTOWNER);
            // See if we are the last active visible popup
            HWND hwndTry;
            while ((hwndTry = User32.GetLastActivePopup(hwndWalk)) != hwndTry)
            {
                if (User32.IsWindowVisible(hwndTry)) break;
                hwndWalk = hwndTry;
            }
            return hwndWalk == Window;
        }

        public void GetInfo()
        {
            GetWindowInfo();
            GetMonitorInfo();
            GetVirtualDesktop();
            GetAppName();
            GetBorders();
        }

        public void GetWindowInfo()
        {
            User32.WINDOWINFO info = new User32.WINDOWINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);
            User32.GetWindowInfo(Window, ref info);
            Info = info;
        }

        public void GetMonitorInfo()
        {
            User32.MONITORINFO monitor = new User32.MONITORINFO();
            monitor.cbSize = (uint)Marshal.SizeOf(monitor);
            MonitorHandle = User32.MonitorFromWindow(Window, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
            User32.GetMonitorInfo(MonitorHandle, ref monitor);
            Monitor = monitor;
        }

        public void GetVirtualDesktop()
        {
            VirtualDesktop virtualDesktop = VirtualDesktop.FromHwnd(Window);
            VirtualDesktop = virtualDesktop;
        }

        public void GetAppName()
        {
            string fileName = "";
            string name = "";
            uint pid = 0;
            User32.GetWindowThreadProcessId(Window, out pid);
            uint capacity = 1024;
            Kernel32.SafeHPROCESS process = Kernel32.OpenProcess(ACCESS_MASK.GENERIC_READ, false, pid);
            StringBuilder stringBuilder = new StringBuilder((int)capacity);
            bool success = Kernel32.QueryFullProcessImageName(process, Kernel32.PROCESS_NAME.PROCESS_NAME_WIN32, stringBuilder, ref capacity);
            if (!success)
            {
                fileName = string.Empty;
            }
            fileName = stringBuilder.ToString();
            switch (fileName)
            {
                case "C:\\Windows\\System32\\ApplicationFrameHost.exe":
                case "":
                    name = GetWindowTitle();
                    break;
                default:
                    FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(fileName);
                    name = myFileVersionInfo.FileDescription;
                    break;
            }
            AppName = name;
        }

        private string GetWindowTitle()
        {
            string windowText = "";
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            if (User32.GetWindowText(Window, Buff, nChars) > 0)
            {
                windowText = Buff.ToString();
            }
            return windowText;
        }

        private void GetBorders()
        {
            Borders.left = Math.Abs(Info.rcWindow.left - Info.rcClient.left);
            Borders.right = Math.Abs(Info.rcWindow.right - Info.rcClient.right);
            Borders.top = Math.Abs(Info.rcWindow.top - Info.rcClient.top);
            Borders.bottom = Math.Abs(Info.rcWindow.bottom - Info.rcClient.bottom);
            if (Info.dwExStyle.HasFlag(User32.WindowStylesEx.WS_EX_WINDOWEDGE))
            {
                Borders.top = 0;
            }
            Console.WriteLine(Borders);
        }

        public override string ToString()
        {
            return $"{AppName}";
        }

        internal Pair<VirtualDesktop, HMONITOR> GetDesktopMonitor()
        {
            return new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop, MonitorHandle);
        }

        public override bool Equals(object obj)
        {
            return obj is DesktopWindow window &&
                   EqualityComparer<VirtualDesktop>.Default.Equals(VirtualDesktop, window.VirtualDesktop) &&
                   EqualityComparer<HMONITOR>.Default.Equals(MonitorHandle, window.MonitorHandle) &&
                   EqualityComparer<HWND>.Default.Equals(Window, window.Window);
        }

        public override int GetHashCode()
        {
            int hashCode = -810864280;
            hashCode = hashCode * -1521134295 + EqualityComparer<VirtualDesktop>.Default.GetHashCode(VirtualDesktop);
            hashCode = hashCode * -1521134295 + MonitorHandle.GetHashCode();
            hashCode = hashCode * -1521134295 + Window.GetHashCode();
            return hashCode;
        }
    }
}
