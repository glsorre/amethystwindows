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
        public void LoadLayouts()
        {
            if (Properties.Settings.Default.Layouts != "")
            {
                ReadLayouts();
            }
        }

        public void SaveLayouts()
        {
            Properties.Settings.Default.Layouts = JsonConvert.SerializeObject(Layouts.ToList(), Formatting.Indented, new LayoutsConverter());
            Properties.Settings.Default.Save();
        }

        public void ReadLayouts()
        {
            Layouts = JsonConvert.DeserializeObject<List<KeyValuePair<Pair<VirtualDesktop, HMONITOR>, Layout>>>(
                Properties.Settings.Default.Layouts.ToString(), new LayoutsConverter()
                ).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public void UpdateLayouts()
        {
            foreach (Pair<VirtualDesktop, HMONITOR> desktopMonitor in Windows.Keys)
            {
                if (!Layouts.ContainsKey(desktopMonitor))
                {
                    Layouts.Add(desktopMonitor, Layout.Tall);
                }
            }
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

        public Layout RotateLayoutClockwise(Pair<VirtualDesktop, HMONITOR> desktopMonitor)
        {
            Layouts[desktopMonitor] = RotateLayoutsClockwise(Layouts[desktopMonitor]);
            return Layouts[desktopMonitor];
        }

        public Layout RotateLayoutCounterClockwise(Pair<VirtualDesktop, HMONITOR> desktopMonitor)
        {
            Layouts[desktopMonitor] = RotateLayoutsCounterClockwise(Layouts[desktopMonitor]);
            return Layouts[desktopMonitor];
        }
    }
}
