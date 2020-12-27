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
using Windows.UI.Popups;
using WindowsDesktop;

namespace AmethystWindowsSystray
{
    class SystrayContext : ApplicationContext
    {
        private AppServiceConnection Connection = null;
        private NotifyIcon NotifyIcon = null;
        private Functions Handlers = null;
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
            MenuItem versionMenuItem = new MenuItem("Amethyst Windows 2020.12 Alpha");
            MenuItem separatorMenuItem = new MenuItem("-");
            MenuItem openMenuItem = new MenuItem("Open", new EventHandler(App_Open));
            MenuItem sendMenuItem = new MenuItem("Refresh", new EventHandler(App_Refresh));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(App_Exit));
            openMenuItem.DefaultItem = true;

            Logger.Information($"inizializing notify icon");
            NotifyIcon = new NotifyIcon();
            NotifyIcon.DoubleClick += new EventHandler(App_Open);
            NotifyIcon.Icon = AmethystWindowsSystray.Properties.Resources.SystrayIcon;
            NotifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { versionMenuItem, separatorMenuItem, openMenuItem, sendMenuItem, exitMenuItem });
            NotifyIcon.Visible = true;

            Logger.Information($"connecting to UWP");
            App_Connect();

            Logger.Information($"generating handlers");
            Handlers = new Functions(new DesktopWindowsManager());
            Handlers.Changed += Handlers_Changed;
            Logger.Information($"getting layouts");
            Handlers.LoadLayouts();
            Logger.Information($"setting hooks");
            Handlers.setWindowsHook();
            Handlers.setKeyboardHook(form.Handle);
            Logger.Information($"getting windows");
            Handlers.GetWindows();
            Logger.Information($"drawing");
            Handlers.DesktopWindowsManager.Draw();
            Logger.Information($"refreshing UWP");
            App_Refresh();

            Logger.Information($"setting virtual desktop change");
            var prova = VirtualDesktop.RegisterListener();
            VirtualDesktop.CurrentChanged += VirtualDesktop_CurrentChanged;

            App_SendPadding();
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
                Handlers.DesktopWindowsManager.Draw();
            }

            if (args.Request.Message.ContainsKey("padding_read"))
            {
                App_SendPadding();
            }

            if (args.Request.Message.ContainsKey("padding_set"))
            {
                args.Request.Message.TryGetValue("padding_set", out object message);
                int newPadding = int.Parse(message.ToString());
                Handlers.DesktopWindowsManager.Padding = newPadding;
                Properties.Settings.Default.Padding = newPadding;
                Properties.Settings.Default.Save();
            }

            deferral.Complete();
        }

        private void VirtualDesktop_CurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
        {
            Handlers.GetWindows();
            Handlers.DesktopWindowsManager.Draw();
            App_Refresh();
        }

        private async Task Form_AmethystSysTrayReconnect_Refresh()
        {
            await Task.Delay(750);
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
            if (e == 0x20) //space bar
            {
                HMONITOR currentMonitor = User32.MonitorFromPoint(Control.MousePosition, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                VirtualDesktop currentDesktop = VirtualDesktop.Current;
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                Handlers.DesktopWindowsManager.Layouts[currentPair] = Handlers.DesktopWindowsManager.RotateLayouts(Handlers.DesktopWindowsManager.Layouts[currentPair]);
                Handlers.DesktopWindowsManager.Draw(currentPair);
                Handlers.DesktopWindowsManager.SaveLayouts();
            }
            if (e == 0x0D) //enter
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = Handlers.DesktopWindowsManager.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                Handlers.DesktopWindowsManager.Windows[currentPair].Move(
                    Handlers.DesktopWindowsManager.Windows[currentPair].IndexOf(selected),
                    0
                    );
                Handlers.DesktopWindowsManager.Draw(currentPair);
            }
            if (e == 0x4A) // j
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = Handlers.DesktopWindowsManager.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                int currentIndex = Handlers.DesktopWindowsManager.Windows[currentPair].IndexOf(selected);
                int maxIndex = Handlers.DesktopWindowsManager.Windows[currentPair].Count - 1;
                if (currentIndex == maxIndex)
                {
                    User32.SetForegroundWindow(Handlers.DesktopWindowsManager.Windows[currentPair][0].Window);
                }
                else
                {
                    User32.SetForegroundWindow(Handlers.DesktopWindowsManager.Windows[currentPair][++currentIndex].Window);
                }
            }
            if (e == 0x4B) // j
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = Handlers.DesktopWindowsManager.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                int currentIndex = Handlers.DesktopWindowsManager.Windows[currentPair].IndexOf(selected);
                int maxIndex = Handlers.DesktopWindowsManager.Windows[currentPair].Count - 1;
                if (currentIndex == 0)
                {
                    User32.SetForegroundWindow(Handlers.DesktopWindowsManager.Windows[currentPair][maxIndex].Window);
                }
                else
                {
                    User32.SetForegroundWindow(Handlers.DesktopWindowsManager.Windows[currentPair][--currentIndex].Window);
                }
            }
            if (e == 0x4C) // l
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = Handlers.DesktopWindowsManager.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                int currentIndex = Handlers.DesktopWindowsManager.Windows[currentPair].IndexOf(selected);
                int maxIndex = Handlers.DesktopWindowsManager.Windows[currentPair].Count - 1;
                if (currentIndex == maxIndex)
                {
                    Handlers.DesktopWindowsManager.Windows[currentPair].Move(currentIndex, 0);
                }
                else
                {
                    Handlers.DesktopWindowsManager.Windows[currentPair].Move(currentIndex, ++currentIndex);
                }
                Handlers.DesktopWindowsManager.Draw(currentPair);
            }
            if (e == 0x48) //h
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = Handlers.DesktopWindowsManager.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                int currentIndex = Handlers.DesktopWindowsManager.Windows[currentPair].IndexOf(selected);
                int maxIndex = Handlers.DesktopWindowsManager.Windows[currentPair].Count - 1;
                if (currentIndex == 0)
                {
                    Handlers.DesktopWindowsManager.Windows[currentPair].Move(currentIndex, maxIndex);
                }
                else
                {
                    Handlers.DesktopWindowsManager.Windows[currentPair].Move(currentIndex, --currentIndex);
                }
                Handlers.DesktopWindowsManager.Draw(currentPair);
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

        private async void App_Refresh()
        {
            ValueSet message = new ValueSet();
            List<List<String>> list = new List<List<String>>();
            foreach (var m in Handlers.DesktopWindowsManager.Windows)
            {
                foreach (var w in m.Value.Select((value, i) => new { i, value }))
                {
                    List<String> item = new List<string>();
                    item.Add(w.value.Window.ToString());
                    item.Add(w.value.AppName);
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
