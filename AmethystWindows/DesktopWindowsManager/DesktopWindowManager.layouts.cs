using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanara.PInvoke;
using WindowsDesktop;

using AmethystWindows.Settings;

namespace AmethystWindows.DesktopWindowsManager
{
    partial class DesktopWindowsManager
    {
        public void LoadLayouts()
        {
            if (MySettings.Instance.Layouts != "[]")
            {
                ReadLayouts();
            }
        }

        public void SaveLayouts()
        {
            MySettings.Instance.Layouts = JsonConvert.SerializeObject(Layouts.ToList(), Formatting.Indented, new LayoutsConverter());
            MySettings.Save();
        }

        public void ReadLayouts()
        {
            Layouts = JsonConvert.DeserializeObject<List<KeyValuePair<Pair<VirtualDesktop, HMONITOR>, Layout>>>(
                MySettings.Instance.Layouts, new LayoutsConverter()
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
