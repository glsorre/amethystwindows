using DesktopMonitorManager.Internal;
using DebounceThrottle;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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
        private DesktopMonitorManager DMM = null;
        private HooksHelper hooksHelper = null;
        private bool Standalone = false;
        public static Logger Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        private DebounceDispatcher debounceDispatcher = new DebounceDispatcher(250);
        MenuItem disableMenuItem;

        public SystrayContext()
        {

        }

        public static SystrayContext Create(bool standalone = false)
        {
            SystrayContext systrayContext = new SystrayContext();
            systrayContext.Initialize(standalone);
            return systrayContext;
        }

        private async void Initialize(bool standalone)
        {
            Logger.Information($"starting (standalone = {standalone})");
            Standalone = standalone;

            Logger.Information($"inizializing transparent form");
            TransparentForm form = new TransparentForm();
            form.AmethystSysTrayReconnect += Form_AmethystSysTrayReconnect;
            form.AmethystSysTrayDisplayChange += Form_AmethystSysTrayDisplayChange;
            form.AmethystSystrayHotKey += Form_AmethystSystrayHotKey;
            MainForm = form;

            Logger.Information($"inizializing context menu");
            MenuItem versionMenuItem = new MenuItem("Amethyst Windows " + Application.ProductVersion);
            versionMenuItem.Enabled = false;
            MenuItem separatorMenuItem1 = new MenuItem("-");
            MenuItem openMenuItem = new MenuItem("Open", new EventHandler(App_Open));
            disableMenuItem = new MenuItem("Disable", new EventHandler(App_Disable));
            MenuItem separatorMenuItem2 = new MenuItem("-");
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(App_Exit));
            openMenuItem.DefaultItem = true;

            Logger.Information($"inizializing notify icon");
            NotifyIcon = new NotifyIcon();
            NotifyIcon.DoubleClick += new EventHandler(App_Open);
            NotifyIcon.Icon = AmethystWindowsSystray.Properties.Resources.SystrayIcon;
            NotifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { versionMenuItem, separatorMenuItem1, openMenuItem, disableMenuItem, separatorMenuItem2, exitMenuItem });
            NotifyIcon.Visible = true;

            Logger.Information($"connecting to UWP");
            await App_Connect();

            Logger.Information($"generating DWM");
            DMM = new DesktopMonitorManager();
            DMM.Changed += Handlers_Changed;
            Logger.Information($"loading desktop monitors");
            DMM.Load();
            Logger.Information($"setting hooks");
            hooksHelper = new HooksHelper(DMM);
            hooksHelper.setWindowsHook();
            hooksHelper.setKeyboardHook(form.Handle);
            Logger.Information($"getting windows");
            DMM.GetWindows();
            Logger.Information($"drawing");
            DMM.Draw();
            
            Logger.Information($"setting virtual desktop change listener");
            IDisposable listener = VirtualDesktop.RegisterListener();
            VirtualDesktop.CurrentChanged += VirtualDesktop_CurrentChanged;

            Logger.Information($"setting virtual desktops");
            int virtualDesktopsExisting = VirtualDesktop.Count;
            int virtualDesktopsToCreate = Properties.Settings.Default.VirtualDesktops - virtualDesktopsExisting;

            if (virtualDesktopsExisting < Properties.Settings.Default.VirtualDesktops)
            {
                for (int i = 1; i <= virtualDesktopsToCreate; i++)
                {
                    VirtualDesktop.Create();
                }
            }
        }

        private void App_Disable(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            if (DMM.Disabled) {
                DMM.Disabled = false;
                menuItem.Checked = false;
                DMM.Draw();
            } else
            {
                DMM.Disabled = true;
                menuItem.Checked = true;
            }
            App_SendPaddingMarginDisabled();
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

            if (args.Request.Message.ContainsKey("exit_confirmed"))
            {
                MainForm.Close();
                Application.Exit();
            }

            if (args.Request.Message.ContainsKey("refresh"))
            {
                App_Refresh();
                App_SendPaddingMarginDisabled();
                App_SendFilters();
            }

            if (args.Request.Message.ContainsKey("redraw"))
            {
                DMM.ClearWindows();
                DMM.GetWindows();
                DMM.Draw();
            }

            if (args.Request.Message.ContainsKey("padding_set"))
            {
                args.Request.Message.TryGetValue("padding_set", out object message);
                int newPadding = int.Parse(message.ToString());
                DMM.Padding = newPadding;
                Properties.Settings.Default.Padding = newPadding;
                Properties.Settings.Default.Save();
            }

            if (args.Request.Message.ContainsKey("margin_top_set"))
            {
                args.Request.Message.TryGetValue("margin_top_set", out object message);
                int newMargin = int.Parse(message.ToString());
                DMM.MarginTop = newMargin;
                Properties.Settings.Default.MarginTop = newMargin;
                Properties.Settings.Default.Save();
            }

            if (args.Request.Message.ContainsKey("margin_bottom_set"))
            {
                args.Request.Message.TryGetValue("margin_bottom_set", out object message);
                int newMargin = int.Parse(message.ToString());
                DMM.MarginBottom = newMargin;
                Properties.Settings.Default.MarginBottom = newMargin;
                Properties.Settings.Default.Save();
            }

            if (args.Request.Message.ContainsKey("margin_left_set"))
            {
                args.Request.Message.TryGetValue("margin_left_set", out object message);
                int newMargin = int.Parse(message.ToString());
                DMM.MarginLeft = newMargin;
                Properties.Settings.Default.MarginLeft = newMargin;
                Properties.Settings.Default.Save();
            }

            if (args.Request.Message.ContainsKey("margin_right_set"))
            {
                args.Request.Message.TryGetValue("margin_right_set", out object message);
                int newMargin = int.Parse(message.ToString());
                DMM.MarginRight = newMargin;
                Properties.Settings.Default.MarginRight = newMargin;
                Properties.Settings.Default.Save();
            }

            if (args.Request.Message.ContainsKey("layout_padding_set"))
            {
                args.Request.Message.TryGetValue("layout_padding_set", out object message);
                int newPadding = int.Parse(message.ToString());
                DMM.LayoutPadding = newPadding;
                Properties.Settings.Default.LayoutPadding = newPadding;
                Properties.Settings.Default.Save();
            }

            if (args.Request.Message.ContainsKey("layout_padding_set"))
            {
                args.Request.Message.TryGetValue("layout_padding_set", out object message);
                int newPadding = int.Parse(message.ToString());
                DMM.LayoutPadding = newPadding;
                Properties.Settings.Default.LayoutPadding = newPadding;
                Properties.Settings.Default.Save();
            }

            if (args.Request.Message.ContainsKey("disable_set"))
            {
                args.Request.Message.TryGetValue("disable_set", out object message);
                bool disabled = bool.Parse(message.ToString());
                DMM.Disabled = disabled;
                disableMenuItem.Checked = disabled;
                if (!disabled)
                {
                    DMM.Draw();
                }
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

                DMM.ConfigurableFilters = parsedFilters;
                Properties.Settings.Default.Filters = JsonConvert.SerializeObject(parsedFilters);
                Properties.Settings.Default.Save();

                DMM.ClearWindows();
                DMM.GetWindows();
                DMM.Draw();
            }

            deferral.Complete();
        }

        private void VirtualDesktop_CurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
        {
            DMM.GetWindows();
            DMM.Draw();
            App_Refresh();
        }

        private async Task Form_AmethystSysTrayReconnect_Refresh()
        {
            await Task.Delay(500);
            App_Refresh();
            App_SendPaddingMarginDisabled();
            App_SendFilters();
        }

        private async void Form_AmethystSysTrayReconnect(object sender, EventArgs e)
        {
            await App_Connect();
            await Form_AmethystSysTrayReconnect_Refresh();
        }

        private void Form_AmethystSysTrayDisplayChange(object sender, EventArgs e)
        {
            DMM.ClearWindows();
            DMM.GetWindows();
            DMM.Draw();
        }

        private void Form_AmethystSystrayHotKey(object sender, int e)
        {
            if (e == 0x11) //space bar
            {
                HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                VirtualDesktop currentDesktop = VirtualDesktop.Current;
                DesktopMonitor currentPair = DMM.FindDesktopMonitor(currentMonitor, currentDesktop);
                AlertForm form = new AlertForm();
                Layout currentLayout = DMM.RotateLayoutClockwise(currentPair);
                User32.MONITORINFO currentMonitorInfo = new User32.MONITORINFO();
                currentMonitorInfo.cbSize = (uint)Marshal.SizeOf(currentMonitorInfo);
                User32.GetMonitorInfo(currentMonitor, ref currentMonitorInfo);
                form.showAlert(currentLayout.ToString(), currentMonitorInfo.rcMonitor.X, currentMonitorInfo.rcMonitor.Width, currentMonitorInfo.rcMonitor.Y, currentMonitorInfo.rcMonitor.Height);
                DMM.Draw(currentPair);
                currentPair.Save();
            }
            if (e == 0x12) //enter
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = DMM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                DesktopMonitor currentPair = DMM.FindDesktopMonitor(currentMonitor, VirtualDesktop.Current);
                DMM.SetMainWindow(currentPair, selected);
                DMM.Draw(currentPair);
            }
            if (e == 0x15) // j
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = DMM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                DesktopMonitor currentPair = DMM.FindDesktopMonitor(currentMonitor, VirtualDesktop.Current);
                DMM.RotateFocusedWindowClockwise(currentPair, selected);
                DMM.Draw(currentPair);
            }
            if (e == 0x14) // k
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = DMM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                DesktopMonitor currentPair = DMM.FindDesktopMonitor(currentMonitor, VirtualDesktop.Current);
                DMM.RotateFocusedWindowCounterClockwise(currentPair, selected);
                DMM.Draw(currentPair);
            }
            if (e == 0x13) // l
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = DMM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                DesktopMonitor currentPair = DMM.FindDesktopMonitor(currentMonitor, VirtualDesktop.Current);
                DMM.MoveWindowClockwise(currentPair, selected);
                DMM.Draw(currentPair);
            }
            if (e == 0x16) //h
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = DMM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                DesktopMonitor currentPair = DMM.FindDesktopMonitor(currentMonitor, VirtualDesktop.Current);
                DMM.MoveWindowCounterClockwise(currentPair, selected);
                DMM.Draw(currentPair);
            }
            if (e == 0x17) //z
            {
                DMM.ClearWindows();
                DMM.GetWindows();
                DMM.Draw();
            }
            if (e == 0x18) //p
            {
                HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                VirtualDesktop currentDesktop = VirtualDesktop.Current;
                DesktopMonitor currentPair = DMM.FindDesktopMonitor(currentMonitor, VirtualDesktop.Current);
                DMM.RotateMonitorClockwise(currentPair);
            }
            if (e == 0x19) //n
            {
                HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                VirtualDesktop currentDesktop = VirtualDesktop.Current;
                DesktopMonitor currentPair = DMM.FindDesktopMonitor(currentMonitor, VirtualDesktop.Current);
                DMM.RotateMonitorCounterClockwise(currentPair);
            }
            if (e == 0x21) //space bar
            {
                HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                VirtualDesktop currentDesktop = VirtualDesktop.Current;
                DesktopMonitor currentPair = DMM.FindDesktopMonitor(currentMonitor, VirtualDesktop.Current);
                AlertForm form = new AlertForm();
                Layout currentLayout = DMM.RotateLayoutCounterClockwise(currentPair);
                User32.MONITORINFO currentMonitorInfo = new User32.MONITORINFO();
                currentMonitorInfo.cbSize = (uint)Marshal.SizeOf(currentMonitorInfo);
                User32.GetMonitorInfo(currentMonitor, ref currentMonitorInfo);
                form.showAlert(currentLayout.ToString(), currentMonitorInfo.rcMonitor.X, currentMonitorInfo.rcMonitor.Width, currentMonitorInfo.rcMonitor.Y, currentMonitorInfo.rcMonitor.Height);
                DMM.Draw(currentPair);
                currentPair.Save();
            }
            if (e == 0x22) //h
            {
                debounceDispatcher.Debounce(() =>
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopMonitor currentPair = DMM.FindDesktopMonitor(currentMonitor, VirtualDesktop.Current);
                    if (currentPair.Count > 1 && (currentPair.Layout == Layout.Tall || currentPair.Layout == Layout.Wide)) {
                        DMM.ExpandMainPane(currentPair);
                        DMM.Draw(currentPair);
                        currentPair.Save();
                    }
                });
            }
            if (e == 0x23) //l
            {
                debounceDispatcher.Debounce(() =>
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    DesktopMonitor currentPair = DMM.FindDesktopMonitor(currentMonitor, VirtualDesktop.Current);
                    if (currentPair.Count > 1 && (currentPair.Layout == Layout.Tall || currentPair.Layout == Layout.Wide))
                    {
                        DMM.ShrinkMainPane(currentPair);
                        DMM.Draw(currentPair);
                        currentPair.Save();
                    }
                });
            }
            if (e == 0x24) //j
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                DesktopWindow selected = DMM.FindWindow(selectedWindow);
                DMM.MoveWindowPreviousScreen(selected);
                DMM.Draw();
            }
            if (e == 0x25) //k
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                DesktopWindow selected = DMM.FindWindow(selectedWindow);
                DMM.MoveWindowNextScreen(selected);
                DMM.Draw();
            }
            if (e == 0x21) //l
            {
                HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                VirtualDesktop currentDesktop = VirtualDesktop.Current;
                DesktopMonitor currentPair = DMM.FindDesktopMonitor(currentMonitor, VirtualDesktop.Current);
                DMM.RotateLayoutCounterClockwise(currentPair);
                DMM.Draw(currentPair);
                currentPair.Save();
            }
            if (e == 0x26) //right
            {
                debounceDispatcher.Debounce(() =>
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = DMM.FindWindow(selectedWindow);
                    DMM.MoveWindowNextVirtualDesktop(selected);
                    DMM.Draw();
                });
            }
            if (e == 0x27) //right
            {
                debounceDispatcher.Debounce(() =>
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = DMM.FindWindow(selectedWindow);
                    DMM.MoveWindowPreviousVirtualDesktop(selected);
                    DMM.Draw();
                });
            }
            if (e == 0x1 || e == 0x2 || e == 0x3 || e == 0x4 || e == 0x5) //1,2,3,4,5
            {
                debounceDispatcher.Debounce(() =>
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = DMM.FindWindow(selectedWindow);
                    DMM.MoveWindowSpecificVirtualDesktop(selected, e - 1);
                    DMM.Draw();
                });
            }
        }

        private async void App_Open(object sender, EventArgs e)
        {
            if (!Standalone)
            {
                IEnumerable<AppListEntry> appListEntries = await Package.Current.GetAppListEntriesAsync();
                await appListEntries.First().LaunchAsync();
            }
        }

        private async void App_SendPaddingMarginDisabled()
        {
            ValueSet message = new ValueSet();
            message.Add("padding_read", Properties.Settings.Default.Padding);
            message.Add("margin_top_read", Properties.Settings.Default.MarginTop);
            message.Add("margin_bottom_read", Properties.Settings.Default.MarginBottom);
            message.Add("margin_left_read", Properties.Settings.Default.MarginLeft);
            message.Add("margin_right_read", Properties.Settings.Default.MarginRight);
            message.Add("layout_padding_read", Properties.Settings.Default.LayoutPadding);
            message.Add("disabled_read", DMM.Disabled);
            await App_Send(message);
        }

        private async void App_SendFilters()
        {
            ValueSet message = new ValueSet();
            List<List<String>> list = new List<List<String>>();
            foreach (var f in DMM.ConfigurableFilters)
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
            foreach (var m in DMM.DesktopMonitors)
            {
                Screen screen = System.Windows.Forms.Screen.AllScreens.First(s => s.Bounds == new Rectangle(m.MonitorInfo.rcMonitor.X, m.MonitorInfo.rcMonitor.Y, m.MonitorInfo.rcMonitor.Width, m.MonitorInfo.rcMonitor.Height));

                foreach (var w in m.Select((value, i) => new { i, value }))
                {
                    List<String> item = new List<string>();
                    item.Add(w.value.Window.DangerousGetHandle().ToString());
                    item.Add(w.value.AppName);
                    item.Add(w.value.ClassName);
                    item.Add(VirtualDesktop.DesktopNameFromDesktop(m.VirtualDesktop));
                    item.Add(screen.DeviceName.ToString().Remove(0,4));
                    item.Add(w.value.Info.rcWindow.ToString());
                    item.Add(w.value.Info.dwStyle.ToString());
                    item.Add(w.value.Info.dwExStyle.ToString());
                    list.Add(item);
                }
            }
            message.Add("refresh", JsonConvert.SerializeObject(list));
            await App_Send(message);
        }

        private async void App_Exit(object sender, EventArgs e)
        {
            if (Connection != null)
            {
                ValueSet message = new ValueSet();
                message.Add("exit", "");
                AppServiceResponse response = await App_Send(message);

                if (response != null)
                {
                    response.Message.TryGetValue("exit_confirmed", out object responseMessage);

                    if ((bool)responseMessage)
                    {
                        if (MainForm.InvokeRequired)
                        {
                            MainForm.Invoke(new MethodInvoker(delegate { MainForm.Close(); }));
                        } else
                        {
                            MainForm.Close();
                        }
                        Application.Exit();          
                    }
                }
            } else
            {
                MainForm.Close();
                Application.Exit();
            }
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

        private async Task<AppServiceResponse> App_Send(ValueSet message)
        {
            if (!Standalone && Connection != null)
            {
                return await Connection.SendMessageAsync(message);
            }
            return null;
        }

        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Connection.ServiceClosed -= Connection_ServiceClosed;
            Connection = null;
        }
    }
}
