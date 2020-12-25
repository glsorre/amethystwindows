using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vanara.PInvoke;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;
using WindowsDesktop;

namespace AmethystWindowsSystray
{
    class SystrayContext : ApplicationContext
    {
        private AppServiceConnection connection = null;
        private NotifyIcon notifyIcon = null;
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
            MenuItem openMenuItem = new MenuItem("Open", new EventHandler(OpenApp));
            MenuItem sendMenuItem = new MenuItem("Refresh", new EventHandler(RefreshUWP));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));
            openMenuItem.DefaultItem = true;

            Logger.Information($"inizializing notify icon");
            notifyIcon = new NotifyIcon();
            notifyIcon.DoubleClick += new EventHandler(OpenApp);
            notifyIcon.Icon = AmethystWindowsSystray.Properties.Resources.SystrayIcon;
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { versionMenuItem, separatorMenuItem, openMenuItem, sendMenuItem, exitMenuItem });
            notifyIcon.Visible = true;

            Logger.Information($"connecting to UWP");
            ConnectToUWP();

            Logger.Information($"generating handlers");
            Handlers = new Functions(new DesktopWindowsManager());
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
            RefreshUWP();

            Logger.Information($"setting virtual desktop change");
            var prova = VirtualDesktop.RegisterListener();
            VirtualDesktop.CurrentChanged += VirtualDesktop_CurrentChanged;
        }

        private void VirtualDesktop_CurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
        {
            Handlers.GetWindows();
            Handlers.DesktopWindowsManager.Draw();
            RefreshUWP();
        }

        private async Task Form_AmethystSysTrayReconnect_Refresh()
        {
            await Task.Delay(750);
            RefreshUWP();
        }

        private async void Form_AmethystSysTrayReconnect(object sender, EventArgs e)
        {
            await ConnectToUWP();
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

        private async void OpenApp(object sender, EventArgs e)
        {
            IEnumerable<AppListEntry> appListEntries = await Package.Current.GetAppListEntriesAsync();
            await appListEntries.First().LaunchAsync();
        }

        private async void RefreshUWP()
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
            message.Add("refresh", JsonSerializer.Serialize(list));
            await SendToUWP(message);
        }

        private void RefreshUWP(object sender, EventArgs e)
        {
            RefreshUWP();
        }

        private async void Exit(object sender, EventArgs e)
        {
            ValueSet message = new ValueSet();
            message.Add("exit", "");
            await SendToUWP(message);
            MainForm.Close();
            Application.Exit();
        }

        private async Task ConnectToUWP()
        {
            if (!Standalone)
            {
                if (connection == null)
                {
                    connection = new AppServiceConnection();
                    connection.PackageFamilyName = Package.Current.Id.FamilyName;
                    connection.AppServiceName = "AmethystWindowsSystray";
                    connection.ServiceClosed += Connection_ServiceClosed;
                    AppServiceConnectionStatus connectionStatus = await connection.OpenAsync();
                }
            }
        }

        private async Task SendToUWP(ValueSet message)
        {
            if (!Standalone)
            {
                await connection.SendMessageAsync(message);
            }
        }

        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            connection.ServiceClosed -= Connection_ServiceClosed;
            connection = null;
        }
    }
}
