using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmethystWindows
{
    public class DesktopWindow
    {
        public string Window { get; set; }
        public string AppName { get; set; }
        public string VirtualDesktop { get; set; }
        public string Monitor { get; set; }
        public string RcWindow { get; set; }
        public string DxStyle { get; set; }
        public string DxExStyle { get; set; }

        public DesktopWindow(string window, string appName, string virtualDesktop, string monitor, string rcWindow, string dxStyle, string dxExStyle)
        {
            Window = window;
            AppName = appName;
            VirtualDesktop = virtualDesktop;
            Monitor = monitor;
            RcWindow = rcWindow;
            DxStyle = dxStyle;
            DxExStyle = dxExStyle;
        }
    }
}
