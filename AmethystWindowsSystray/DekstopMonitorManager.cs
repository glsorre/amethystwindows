using DesktopMonitorManager.Internal;
using DebounceThrottle;
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
using System.Windows.Forms;
using Vanara.PInvoke;
using WindowsDesktop;

[assembly: InternalsVisibleTo("AmethystWindowsSystrayTests")]
namespace AmethystWindowsSystray
{
    partial class DesktopMonitorManager
    {
        public List<DesktopMonitor> DesktopMonitors { get; }

        public event EventHandler<string> Changed;

        private DesktopWindowComparer DesktopWindowComparer = new DesktopWindowComparer();

        private const int debounceMillis = 400;
        private DebounceDispatcher DebounceDispatcherAdd = new DebounceDispatcher(debounceMillis);
        private DebounceDispatcher DebounceDispatcherRemove = new DebounceDispatcher(debounceMillis);

        private readonly string[] FixedFilters = new string[] {
            "Amethyst Windows",
            "AmethystWindowsPackaging",
            "Cortana",
            "Microsoft Spy++",
            "Task Manager",
        };

        public List<Pair<string, string>> ConfigurableFilters = new List<Pair<string, string>>();

        private int padding;

        public int Padding
        {
            get { return padding; }
            set
            {
                padding = value;
                Draw();
            }
        }

        private int marginTop;

        public int MarginTop
        {
            get { return marginTop; }
            set
            {
                marginTop = value;
                Draw();
            }
        }

        private int marginBottom;

        public int MarginBottom
        {
            get { return marginBottom; }
            set
            {
                marginBottom = value;
                Draw();
            }
        }

        private int marginLeft;

        public int MarginLeft
        {
            get { return marginLeft; }
            set
            {
                marginLeft = value;
                Draw();
            }
        }

        private int marginRight;

        public int MarginRight
        {
            get { return marginRight; }
            set
            {
                marginRight = value;
                Draw();
            }
        }

        private int layoutPadding;

        public int LayoutPadding
        {
            get { return layoutPadding; }
            set
            {
                layoutPadding = value;
                Draw();
            }
        }

        private bool disabled;

        public bool Disabled
        {
            get { return disabled; }
            set
            {
                disabled = value;
            }
        }

        public DesktopMonitorManager()
        {
            this.padding = Properties.Settings.Default.Padding;
            this.marginTop = Properties.Settings.Default.MarginTop;
            this.marginBottom = Properties.Settings.Default.MarginBottom;
            this.marginLeft = Properties.Settings.Default.MarginLeft;
            this.marginRight = Properties.Settings.Default.MarginRight;
            this.layoutPadding = Properties.Settings.Default.LayoutPadding;
            this.ConfigurableFilters = JsonConvert.DeserializeObject<List<Pair<string, string>>>(Properties.Settings.Default.Filters);
            this.DesktopMonitors = new List<DesktopMonitor>();
        }

        public void Load()
        {
            if (Properties.Settings.Default.DesktopMonitors != null)
            {
                foreach (string desktopMonitor in Properties.Settings.Default.DesktopMonitors)
                {
                    DesktopMonitors.Add(JsonConvert.DeserializeObject<DesktopMonitor>(
                        desktopMonitor, new DesktopMonitorConverter()
                    ));
                }
            }
        }

        public DesktopMonitor FindDesktopMonitor(HMONITOR monitor, VirtualDesktop virtualDesktop)
        {
            int index = DesktopMonitors.IndexOf(new DesktopMonitor(monitor, virtualDesktop));
            if (index != -1)
            {
                return DesktopMonitors[index];
            }
            else
            {
                return null;
            }
        }

        public DesktopMonitor FindDesktopMonitor(DesktopMonitor desktopMonitor)
        {
            int index = DesktopMonitors.IndexOf(desktopMonitor);
            if (index != -1)
            {
                return DesktopMonitors[index];
            }
            else
            {
                return null;
            }
        }

        public DesktopMonitor FindDesktopMonitor(DesktopWindow desktopWindow)
        {
            DesktopMonitor desktopMonitor = DesktopMonitors.FirstOrDefault((dM) => dM.Contains(desktopWindow));
            return desktopMonitor;
        }

        public void Draw()
        {
            foreach (DesktopMonitor desktopMonitor in DesktopMonitors)
            {
                desktopMonitor.Draw(new MarginPaddingStruct(
                    MarginTop,
                    MarginBottom,
                    MarginLeft,
                    MarginRight,
                    LayoutPadding,
                    Padding
                ));
            }
        }

        public void Draw(DesktopMonitor desktopMonitor) 
        {
            desktopMonitor.Draw(new MarginPaddingStruct(
                MarginTop,
                MarginBottom,
                MarginLeft,
                MarginRight,
                LayoutPadding,
                Padding
            ));
        }

        public void RotateMonitorClockwise(DesktopMonitor currentDesktopMonitor)
        {
            List<HMONITOR> virtualDesktopMonitors = DesktopMonitors
                .Where(desktopMonitor => desktopMonitor.VirtualDesktop.Equals(currentDesktopMonitor.VirtualDesktop))
                .Select(desktopMonitor => desktopMonitor.Monitor)
                .ToList();

            HMONITOR nextMonitor = virtualDesktopMonitors.SkipWhile(x => x != currentDesktopMonitor.Monitor).Skip(1).DefaultIfEmpty(virtualDesktopMonitors[0]).FirstOrDefault();
            DesktopMonitor nextDesktopMonitor = FindDesktopMonitor(nextMonitor, currentDesktopMonitor.VirtualDesktop);

            User32.SetForegroundWindow(nextDesktopMonitor.FirstOrDefault().Window);
        }

        public void RotateMonitorCounterClockwise(DesktopMonitor currentDesktopMonitor)
        {
            List<HMONITOR> virtualDesktopMonitors = DesktopMonitors
                .Where(desktopMonitor => desktopMonitor.VirtualDesktop.Equals(currentDesktopMonitor.VirtualDesktop))
                .Select(desktopMonitor => desktopMonitor.Monitor)
                .ToList();

            HMONITOR nextMonitor = virtualDesktopMonitors.TakeWhile(x => x != currentDesktopMonitor.Monitor).Skip(1).DefaultIfEmpty(virtualDesktopMonitors[0]).FirstOrDefault();
            DesktopMonitor nextDesktopMonitor = FindDesktopMonitor(nextMonitor, currentDesktopMonitor.VirtualDesktop);

            User32.SetForegroundWindow(nextDesktopMonitor.FirstOrDefault().Window);
        }

        public void ShrinkMainPane(DesktopMonitor key)
        {
            key.Factor++;
        }

        public void ExpandMainPane(DesktopMonitor key)
        {
            key.Factor--;
        }

        private Layout RotateLayoutsClockwise(Layout currentLayout)
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

        private Layout RotateLayoutsCounterClockwise(Layout currentLayout)
        {
            IEnumerable<Layout> values = Enum.GetValues(typeof(Layout)).Cast<Layout>();
            if (currentLayout == 0)
            {
                return Layout.Tall;
            }
            else
            {
                return --currentLayout;
            }
        }

        public Layout RotateLayoutClockwise(DesktopMonitor desktopMonitor)
        {
            desktopMonitor.Layout = RotateLayoutsClockwise(desktopMonitor.Layout);
            return desktopMonitor.Layout;
        }

        public Layout RotateLayoutCounterClockwise(DesktopMonitor desktopMonitor)
        {
            desktopMonitor.Layout = RotateLayoutsCounterClockwise(desktopMonitor.Layout);
            return desktopMonitor.Layout;
        }
    }
}