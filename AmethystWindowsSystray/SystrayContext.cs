using DesktopWindowManager.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vanara.PInvoke;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;
using WindowsDesktop;
using WindowsDesktop.Internal;

namespace AmethystWindowsSystray
{
    class SystrayContext : ApplicationContext
    {
        private AppServiceConnection Connection = null;
        private NotifyIcon NotifyIcon = null;
        private DesktopWindowsManager DWM = null;
        private HooksHelper hooksHelper = null;
        private bool Standalone = false;
        public static Logger Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

        public SystrayContext(bool standalone = false)
        {
            Logger.Information($"starting (standalone = {standalone})");
            Standalone = standalone;

            Logger.Information($"inizializing transparent form");
            TransparentForm form = new TransparentForm();
            form.AmethystSysTrayReconnect += Form_AmethystSysTrayReconnect;
            form.AmethystSystrayHotKey += Form_AmethystSystrayHotKey;
            MainForm = form;

            Logger.Information($"inizializing context menu");
            MenuItem versionMenuItem = new MenuItem("Amethyst Windows");
            versionMenuItem.Enabled = false;
            MenuItem separatorMenuItem = new MenuItem("-");
            MenuItem openMenuItem = new MenuItem("Open", new EventHandler(App_Open));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(App_Exit));
            openMenuItem.DefaultItem = true;

            Logger.Information($"inizializing notify icon");
            NotifyIcon = new NotifyIcon();
            NotifyIcon.DoubleClick += new EventHandler(App_Open);
            NotifyIcon.Icon = AmethystWindowsSystray.Properties.Resources.SystrayIcon;
            NotifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { versionMenuItem, separatorMenuItem, openMenuItem, exitMenuItem });
            NotifyIcon.Visible = true;

            Logger.Information($"connecting to UWP");
            App_Connect();

            Logger.Information($"generating DWM");
            DWM = new DesktopWindowsManager();
            DWM.Changed += Handlers_Changed;
            Logger.Information($"getting layouts");
            DWM.LoadLayouts();
            Logger.Information($"setting hooks");
            hooksHelper = new HooksHelper(DWM);
            hooksHelper.setWindowsHook();
            hooksHelper.setKeyboardHook(form.Handle);
            Logger.Information($"getting windows");
            DWM.GetWindows();
            Logger.Information($"drawing");
            DWM.Draw();
            
            Logger.Information($"setting virtual desktop change listener");
            var prova = VirtualDesktop.RegisterListener();
            VirtualDesktop.CurrentChanged += VirtualDesktop_CurrentChanged;

            Logger.Information($"refreshing UWP");
            App_Refresh();
            App_SendPadding();
            App_SendFilters();
        }

        private void Handlers_Changed(object sender, string e)
        {
            if (Connection != null) {
                App_Refresh();
            }
        }

        private void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            AppServiceDeferral deferral = args.GetDeferral();

            if (args.Request.Message.ContainsKey("refresh"))
            {
                App_Refresh();
            }

            if (args.Request.Message.ContainsKey("redraw"))
            {
                DWM.Draw();
            }

            if (args.Request.Message.ContainsKey("padding_read"))
            {
                App_SendPadding();
            }

            if (args.Request.Message.ContainsKey("padding_set"))
            {
                args.Request.Message.TryGetValue("padding_set", out object message);
                int newPadding = int.Parse(message.ToString());
                DWM.Padding = newPadding;
                Properties.Settings.Default.Padding = newPadding;
                Properties.Settings.Default.Save();
            }

            if (args.Request.Message.ContainsKey("filters_set"))
            {
                args.Request.Message.TryGetValue("filters_set", out object message);
                List<List<string>> receivedFilters = JsonConvert.DeserializeObject<List<List<string>>>(message.ToString());
                List<Pair<string, string>> parsedFilters = new List<Pair<string, string>>();

                foreach (List<string> f in receivedFilters)
                {
                    parsedFilters.Add(new Pair<string, string>(
                        f[0],
                        f[1]
                        ));
                }

                DWM.ConfigurableFilters = parsedFilters;
                Properties.Settings.Default.Filters = JsonConvert.SerializeObject(parsedFilters);
                Properties.Settings.Default.Save();

                DWM.ClearWindows();
                DWM.GetWindows();
                DWM.Draw();
            }

            deferral.Complete();
        }

        private void VirtualDesktop_CurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
        {
            DWM.GetWindows();
            DWM.Draw();
            App_Refresh();
        }

        private async Task Form_AmethystSysTrayReconnect_Refresh()
        {
            await Task.Delay(500);
            App_Refresh();
            App_SendPadding();
        }

        private async void Form_AmethystSysTrayReconnect(object sender, EventArgs e)
        {
            await App_Connect();
            await Form_AmethystSysTrayReconnect_Refresh();
        }

        private void Form_AmethystSystrayHotKey(object sender, int e)
        {
            Console.WriteLine(e);

            if (e == 0x20) //space bar
            {
                HMONITOR currentMonitor = User32.MonitorFromPoint(Control.MousePosition, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                VirtualDesktop currentDesktop = VirtualDesktop.Current;
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                DWM.RotateLayout(currentPair);
                DWM.Draw(currentPair);
                DWM.SaveLayouts();
            }
            if (e == 0x0D) //enter
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                DWM.SetMainWindow(currentPair, selected);
                DWM.Draw(currentPair);
            }
            if (e == 0x4A) // j
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                DWM.RotateFocusedWindowClockwise(currentPair, selected);
                DWM.Draw(currentPair);
            }
            if (e == 0x4B) // j
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                DWM.RotateFocusedWindowCounterClockwise(currentPair, selected);
                DWM.Draw(currentPair);
            }
            if (e == 0x4C) // l
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                DWM.MoveWindowClockwise(currentPair, selected);
                DWM.Draw(currentPair);
            }
            if (e == 0x48) //h
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                DWM.MoveWindowCounterClockwise(currentPair, selected);
                DWM.Draw(currentPair);
            }
            if (e == 0x0) //win + h
            {
                HMONITOR currentMonitor = User32.MonitorFromPoint(Control.MousePosition, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                DWM.ExpandMainPane(currentPair);
                DWM.Draw(currentPair);
            }
            if (e == 0x1) //win + l
            {
                HMONITOR currentMonitor = User32.MonitorFromPoint(Control.MousePosition, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                DWM.ShrinkMainPane(currentPair);
                DWM.Draw(currentPair);
            }
        }

        private async void App_Open(object sender, EventArgs e)
        {
            IEnumerable<AppListEntry> appListEntries = await Package.Current.GetAppListEntriesAsync();
            await appListEntries.First().LaunchAsync();
        }

        private async void App_SendPadding()
        {
            ValueSet message = new ValueSet();
            message.Add("padding_read", Properties.Settings.Default.Padding);
            await App_Send(message);
        }

        private async void App_SendFilters()
        {
            ValueSet message = new ValueSet();
            List<List<String>> list = new List<List<String>>();
            foreach (var f in DWM.ConfigurableFilters)
            {
                List<String> item = new List<string>();
                item.Add(f.Item1);
                item.Add(f.Item2);
                list.Add(item);        
            }
            message.Add("filters_read", JsonConvert.SerializeObject(list));
            await App_Send(message);
        }

        private async void App_Refresh()
        {
            ValueSet message = new ValueSet();
            List<List<String>> list = new List<List<String>>();
            foreach (var m in DWM.Windows)
            {
                foreach (var w in m.Value.Select((value, i) => new { i, value }))
                {
                    List<String> item = new List<string>();
                    item.Add(w.value.Window.ToString());
                    item.Add(w.value.AppName);
                    item.Add(w.value.ClassName);
                    item.Add(w.value.VirtualDesktop.ToString());
                    item.Add(w.value.Monitor.rcMonitor.ToString());
                    item.Add(w.value.Info.rcWindow.ToString());
                    item.Add(w.value.Info.dwStyle.ToString());
                    item.Add(w.value.Info.dwExStyle.ToString());
                    list.Add(item);
                }
            }
            message.Add("refresh", JsonConvert.SerializeObject(list));
            await App_Send(message);
        }

        private void App_Refresh(object sender, EventArgs e)
        {
            App_Refresh();
        }

        private async void App_Exit(object sender, EventArgs e)
        {
            if (Connection != null)
            {
                ValueSet message = new ValueSet();
                message.Add("exit", "");
                await App_Send(message);
            }
            MainForm.Close();
            Application.Exit();
        }

        private async Task App_Connect()
        {
            if (!Standalone)
            {
                if (Connection == null)
                {
                    Connection = new AppServiceConnection();
                    Connection.PackageFamilyName = Package.Current.Id.FamilyName;
                    Connection.AppServiceName = "AmethystWindowsSystray";
                    Connection.ServiceClosed += Connection_ServiceClosed;
                    Connection.RequestReceived += Connection_RequestReceived;
                    AppServiceConnectionStatus connectionStatus = await Connection.OpenAsync();
                }
            }
        }

        private async Task App_Send(ValueSet message)
        {
            if (!Standalone)
            {
                await Connection.SendMessageAsync(message);
            }
        }

        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Connection.ServiceClosed -= Connection_ServiceClosed;
            Connection = null;
        }
    }
}
