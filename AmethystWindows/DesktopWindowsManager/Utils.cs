using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
        public K Key { get; set; }
        public V Value { get; set; }

        public Pair(K key, V value)
        {
            Key = key;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Pair<K, V> pair &&
                   EqualityComparer<K>.Default.Equals(Key, pair.Key) &&
                   EqualityComparer<V>.Default.Equals(Value, pair.Value);
        }

        public override int GetHashCode()
        {
            int hashCode = -1030903623;
            hashCode = hashCode * -1521134295 + EqualityComparer<K>.Default.GetHashCode(Key);
            hashCode = hashCode * -1521134295 + EqualityComparer<V>.Default.GetHashCode(Value);
            return hashCode;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    public class DesktopMonitorsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(List<ViewModelDesktopMonitor>);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var list = value as List<ViewModelDesktopMonitor>;

            writer.WriteStartArray();
            foreach (ViewModelDesktopMonitor desktopMonitor in list)
            {
                User32.MONITORINFO info = new User32.MONITORINFO();
                info.cbSize = (uint)Marshal.SizeOf(info);
                User32.GetMonitorInfo(desktopMonitor.Monitor, ref info);

                writer.WriteStartObject();
                writer.WritePropertyName("DesktopID");
                writer.WriteValue(desktopMonitor.VirtualDesktop.Id);
                writer.WritePropertyName("MonitorX");
                writer.WriteValue(info.rcMonitor.X);
                writer.WritePropertyName("MonitorY");
                writer.WriteValue(info.rcMonitor.Y);
                writer.WritePropertyName("Layout");
                writer.WriteValue(desktopMonitor.Layout);
                writer.WritePropertyName("Factor");
                writer.WriteValue(desktopMonitor.Factor);
                writer.WriteEndObject();

            }
            writer.WriteEndArray();

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<ViewModelDesktopMonitor> list = new List<ViewModelDesktopMonitor>();

            JArray array = JArray.Load(reader);

            foreach (JObject desktopMonitor in array.Children())
            {
                Layout layout = (Layout)desktopMonitor.GetValue("Layout").Value<int>();
                int factor = desktopMonitor.GetValue("Layout").Value<int>();

                Point point = new Point(desktopMonitor.GetValue("MonitorX").Value<int>() + 100, desktopMonitor.GetValue("MonitorY").Value<int>() + 100);
                HMONITOR monitor = User32.MonitorFromPoint(point, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);

                VirtualDesktop savedDesktop = VirtualDesktop.GetDesktops().First(vD => vD.Id.Equals(new Guid(desktopMonitor.GetValue("DesktopID").Value<string>())));
                HMONITOR savedMonitor = monitor;

                list.Add(new ViewModelDesktopMonitor(monitor, savedDesktop, factor, layout));
            }

            return list;
        }
    }
}
