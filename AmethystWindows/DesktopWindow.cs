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
        public string rcWindow { get; set; }
        public string dxStyle { get; set; }
        public string dxExStyle { get; set; }

        public DesktopWindow(string window, string appName, string virtualDesktop, string monitor, string rcWindow, string dxStyle, string dxExStyle)
        {
            Window = window;
            AppName = appName;
            VirtualDesktop = virtualDesktop;
            Monitor = monitor;
            this.rcWindow = rcWindow;
            this.dxStyle = dxStyle;
            this.dxExStyle = dxExStyle;
        }
    }
}
