using DesktopMonitorManager.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using WindowsDesktop;

namespace DesktopMonitorManager.Internal
{
    class DesktopWindow
    {
        public HWND Window { get; set; }
        public User32.WINDOWINFO Info { get; set; }
        public string AppName { get; set; }
        public RECT Borders;
        public string ClassName { get; set; }
        public bool IsUWP { get; set; }

        private static readonly string[] WindowsClassNamesToSkip =
        {
            "Shell_TrayWnd",
            "DV2ControlHost",
            "MsgrIMEWindowClass",
            "SysShadow",
            "Button",
            "ConsoleWindowClass",
        };

        public DesktopWindow(HWND window)
        {
            Window = window;
            IsUWP = false;
            GetWindowInfo();
            GetClassName();
        }

        public bool IsRuntimePresent()
        {
            bool common = User32.IsWindowVisible(Window) &&
                    !User32.IsIconic(Window) &&
                    IsAltTabWindow() &&
                    HasMinDimension() &&
                    !IsTransparent() &&
                    Info.dwExStyle.HasFlag(User32.WindowStylesEx.WS_EX_WINDOWEDGE) &&
                    Info.dwStyle.HasFlag(User32.WindowStyles.WS_MAXIMIZEBOX) &&
                    Info.dwStyle.HasFlag(User32.WindowStyles.WS_MINIMIZEBOX) &&
                    !Info.dwExStyle.HasFlag(User32.WindowStylesEx.WS_EX_TOPMOST);

            if (IsUWP)
            {
                return common &&
                    !IsBackgroundAppWindow();
            } 
            else
            {
                return common &&
                    !Info.dwExStyle.HasFlag(User32.WindowStylesEx.WS_EX_TOOLWINDOW);
            }  
        }

        private bool IsTransparent()
        {
            if (Info.dwExStyle.HasFlag(User32.WindowStylesEx.WS_EX_TRANSPARENT)) return true;
            return false;
        }

        private bool HasMinDimension()
        {
            if (Info.rcClient.Width < 10 && Info.rcClient.Height < 10) return false;
            return true;
        }

        public bool IsRuntimeValuable()
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
            GetAppName();
            GetClassName();
            GetBorders();
        }

        public void GetWindowInfo()
        {
            User32.WINDOWINFO info = new User32.WINDOWINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);
            User32.GetWindowInfo(Window, ref info);
            Info = info;
        }

        public DesktopMonitor GetDesktopMonitor()
        {
            HMONITOR monitor = User32.MonitorFromWindow(Window, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);

            VirtualDesktop virtualDesktop = VirtualDesktop.Current;
            try
            {
                virtualDesktop = VirtualDesktop.FromHwnd(Window);
            }
            catch
            {
                virtualDesktop = VirtualDesktop.Current;
            }

            return new DesktopMonitor(monitor, virtualDesktop);
        }

        public void GetClassName()
        {
            uint capacity = 1024;
            StringBuilder stringBuilder = new StringBuilder((int)capacity);
            User32.GetClassName(Window, stringBuilder, (int)capacity);
            ClassName = stringBuilder.ToString();
            switch (ClassName)
            {
                case "ApplicationFrameWindow":
                    IsUWP = true;
                    break;
            }
        }

        public void GetAppName()
        {
            string fileName = string.Empty;
            string name = string.Empty;
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
                    try
                    {
                        FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(fileName);
                        name = myFileVersionInfo.FileDescription;
                    } catch (FileNotFoundException e)
                    {
                        name = fileName.Split(new string[] { "\\" }, StringSplitOptions.None).Last();
                    }
                    break;
            }
            if (name == null)
            {
                name = fileName.Split(new string[] { "\\" }, StringSplitOptions.None).Last();
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
        }

        public override string ToString()
        {
            return $"{AppName}";
        }

        public override bool Equals(object obj)
        {
            return obj is DesktopWindow window &&
                   EqualityComparer<HWND>.Default.Equals(Window, window.Window) &&
                   IsUWP == window.IsUWP;
        }

        public override int GetHashCode()
        {
            int hashCode = -1305339548;
            hashCode = hashCode * -1521134295 + Window.GetHashCode();
            hashCode = hashCode * -1521134295 + IsUWP.GetHashCode();
            return hashCode;
        }
    }
}
