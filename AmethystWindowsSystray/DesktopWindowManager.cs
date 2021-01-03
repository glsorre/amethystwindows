﻿using DesktopWindowManager.Internal;
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
        public Dictionary<Pair<VirtualDesktop, HMONITOR>, Layout> Layouts;
        public Dictionary<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>> Windows;
        public Dictionary<Pair<VirtualDesktop, HMONITOR>, bool> WindowsSubcribed;
        public event EventHandler<string> Changed;

        private readonly string[] FixedFilters = new string[] {
            "Amethyst Windows",
            "AmethystWindowsPackaging",
            "Task Manager" };
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

        public DesktopWindowsManager()
        {
            this.padding = Properties.Settings.Default.Padding;
            this.ConfigurableFilters = JsonConvert.DeserializeObject<List<Pair<string, string>>>(Properties.Settings.Default.Filters);
            this.Layouts = new Dictionary<Pair<VirtualDesktop, HMONITOR>, Layout>();
            this.Windows = new Dictionary<Pair<VirtualDesktop, HMONITOR>, ObservableCollection<DesktopWindow>>();
            this.WindowsSubcribed = new Dictionary<Pair<VirtualDesktop, HMONITOR>, bool>();
        }

    }
}
