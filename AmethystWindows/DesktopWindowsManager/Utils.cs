using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using WindowsDesktop;

namespace AmethystWindows.DesktopWindowsManager
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

        public override string ToString()
        {
            return base.ToString();
        }
    }

    public class LayoutsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(List<KeyValuePair<Pair<VirtualDesktop, HMONITOR>, Layout>>);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var list = value as List<KeyValuePair<Pair<VirtualDesktop, HMONITOR>, Layout>>;

            writer.WriteStartArray();
            foreach (KeyValuePair<Pair<VirtualDesktop, HMONITOR>, Layout> pair in list)
            {
                User32.MONITORINFOEX info = new User32.MONITORINFOEX();
                info.cbSize = (uint)Marshal.SizeOf(info);
                User32.GetMonitorInfo(pair.Key.Item2, ref info);

                string desktopLabel = string.IsNullOrEmpty(pair.Key.Item1.Name) ? pair.Key.Item1.Id.ToString() : pair.Key.Item1.Name;

                writer.WriteStartObject();
                writer.WritePropertyName("Desktop");
                writer.WriteValue(desktopLabel);
                writer.WritePropertyName("MonitorX");
                writer.WriteValue(info.rcMonitor.X);
                writer.WritePropertyName("MonitorY");
                writer.WriteValue(info.rcMonitor.Y);
                writer.WritePropertyName("Layout");
                writer.WriteValue((int)pair.Value);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<KeyValuePair<Pair<VirtualDesktop, HMONITOR>, Layout>> list = new List<KeyValuePair<Pair<VirtualDesktop, HMONITOR>, Layout>>();

            JArray array = JArray.Load(reader);

            VirtualDesktop virtualDesktop = null;
            HMONITOR hMONITOR = HMONITOR.NULL;
            Layout layout = Layout.Tall;

            foreach (JObject desktopMonitor in array.Children())
            {
                var properties = desktopMonitor.Properties().ToList();
                Point point = new Point(properties[1].Value.ToObject<int>() + 100, properties[2].Value.ToObject<int>() + 100);
                HMONITOR monitor = User32.MonitorFromPoint(point, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);

                try
                {
                    virtualDesktop = VirtualDesktop.GetDesktops().First(vD => vD.Name.Equals(properties[0].Value.ToString()));
                } catch {
                    virtualDesktop = VirtualDesktop.GetDesktops().First(vD => vD.Id.ToString().Equals(properties[0].Value.ToString()));
                }
                hMONITOR = monitor;
                layout = (Layout)properties[3].Value.ToObject<int>();
            }

            var key = new Pair<VirtualDesktop, HMONITOR>(virtualDesktop, hMONITOR);
            list.Add(new KeyValuePair<Pair<VirtualDesktop, HMONITOR>, Layout>(key, layout));

            return list;
        }
    }

    public class FactorsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(List<KeyValuePair<Pair<VirtualDesktop, HMONITOR>, int>>);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var list = value as List<KeyValuePair<Pair<VirtualDesktop, HMONITOR>, int>>;

            writer.WriteStartArray();
            foreach (KeyValuePair<Pair<VirtualDesktop, HMONITOR>, int> pair in list)
            {
                User32.MONITORINFOEX info = new User32.MONITORINFOEX();
                info.cbSize = (uint)Marshal.SizeOf(info);
                User32.GetMonitorInfo(pair.Key.Item2, ref info);

                writer.WriteStartObject();
                writer.WritePropertyName("Desktop");
                writer.WriteValue(pair.Key.Item1.Name);
                writer.WritePropertyName("MonitorX");
                writer.WriteValue(info.rcMonitor.X);
                writer.WritePropertyName("MonitorY");
                writer.WriteValue(info.rcMonitor.Y);
                writer.WritePropertyName("Factor");
                writer.WriteValue(pair.Value);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<KeyValuePair<Pair<VirtualDesktop, HMONITOR>, int>> list = new List<KeyValuePair<Pair<VirtualDesktop, HMONITOR>, int>>();

            JArray array = JArray.Load(reader);

            VirtualDesktop virtualDesktop = null;
            HMONITOR hMONITOR = HMONITOR.NULL;
            int factor = 0;

            foreach (JObject desktopMonitor in array.Children())
            {
                var properties = desktopMonitor.Properties().ToList();
                Point point = new Point(properties[1].Value.ToObject<int>() + 100, properties[2].Value.ToObject<int>() + 100);
                HMONITOR monitor = User32.MonitorFromPoint(point, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);

                virtualDesktop = VirtualDesktop.GetDesktops().First(vD => vD.Name.Equals(properties[0].Value.ToString()));
                hMONITOR = monitor;
                factor = properties[3].Value.ToObject<int>();
            }

            var key = new Pair<VirtualDesktop, HMONITOR>(virtualDesktop, hMONITOR);
            list.Add(new KeyValuePair<Pair<VirtualDesktop, HMONITOR>, int>(key, factor));

            return list;
        }
    }
}
