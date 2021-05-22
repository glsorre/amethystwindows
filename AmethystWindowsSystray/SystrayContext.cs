using DebounceThrottle;
using DesktopWindowManager.Internal;
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
        private DesktopWindowsManager DWM = null;
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
            DWM = new DesktopWindowsManager();
            DWM.Changed += Handlers_Changed;
            Logger.Information($"getting layouts");
            DWM.LoadLayouts();
            DWM.LoadFactors();
            Logger.Information($"setting hooks");
            hooksHelper = new HooksHelper(DWM);
            hooksHelper.setWindowsHook();
            hooksHelper.setKeyboardHook(form.Handle);
            Logger.Information($"getting windows");
            DWM.GetWindows();
            Logger.Information($"drawing");
            DWM.Draw();
            
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
            if (DWM.Disabled) {
                DWM.Disabled = false;
                menuItem.Checked = false;
                DWM.Draw();
            } else
            {
                DWM.Disabled = true;
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
                DWM.ClearWindows();
                DWM.GetWindows();
                DWM.Draw();
            }

            if (args.Request.Message.ContainsKey("padding_set"))
            {
                args.Request.Message.TryGetValue("padding_set", out object message);
                int newPadding = int.Parse(message.ToString());
                DWM.Padding = newPadding;
                Properties.Settings.Default.Padding = newPadding;
                Properties.Settings.Default.Save();
            }

            if (args.Request.Message.ContainsKey("margin_top_set"))
            {
                args.Request.Message.TryGetValue("margin_top_set", out object message);
                int newMargin = int.Parse(message.ToString());
                DWM.MarginTop = newMargin;
                Properties.Settings.Default.MarginTop = newMargin;
                Properties.Settings.Default.Save();
            }

            if (args.Request.Message.ContainsKey("margin_bottom_set"))
            {
                args.Request.Message.TryGetValue("margin_bottom_set", out object message);
                int newMargin = int.Parse(message.ToString());
                DWM.MarginBottom = newMargin;
                Properties.Settings.Default.MarginBottom = newMargin;
                Properties.Settings.Default.Save();
            }

            if (args.Request.Message.ContainsKey("margin_left_set"))
            {
                args.Request.Message.TryGetValue("margin_left_set", out object message);
                int newMargin = int.Parse(message.ToString());
                DWM.MarginLeft = newMargin;
                Properties.Settings.Default.MarginLeft = newMargin;
                Properties.Settings.Default.Save();
            }

            if (args.Request.Message.ContainsKey("margin_right_set"))
            {
                args.Request.Message.TryGetValue("margin_right_set", out object message);
                int newMargin = int.Parse(message.ToString());
                DWM.MarginRight = newMargin;
                Properties.Settings.Default.MarginRight = newMargin;
                Properties.Settings.Default.Save();
            }

            if (args.Request.Message.ContainsKey("layout_padding_set"))
            {
                args.Request.Message.TryGetValue("layout_padding_set", out object message);
                int newPadding = int.Parse(message.ToString());
                DWM.LayoutPadding = newPadding;
                Properties.Settings.Default.LayoutPadding = newPadding;
                Properties.Settings.Default.Save();
            }

            if (args.Request.Message.ContainsKey("layout_padding_set"))
            {
                args.Request.Message.TryGetValue("layout_padding_set", out object message);
                int newPadding = int.Parse(message.ToString());
                DWM.LayoutPadding = newPadding;
                Properties.Settings.Default.LayoutPadding = newPadding;
                Properties.Settings.Default.Save();
            }

            if (args.Request.Message.ContainsKey("disable_set"))
            {
                args.Request.Message.TryGetValue("disable_set", out object message);
                bool disabled = bool.Parse(message.ToString());
                DWM.Disabled = disabled;
                disableMenuItem.Checked = disabled;
                if (!disabled)
                {
                    DWM.Draw();
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
            DWM.ClearWindows();
            DWM.GetWindows();
            DWM.Draw();
        }

        private void Form_AmethystSystrayHotKey(object sender, int e)
        {
            if (e == 0x11) //space bar
            {
                HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                VirtualDesktop currentDesktop = VirtualDesktop.Current;
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                AlertForm form = new AlertForm();
                Layout currentLayout = DWM.RotateLayoutClockwise(currentPair);
                User32.MONITORINFO currentMonitorInfo = new User32.MONITORINFO();
                currentMonitorInfo.cbSize = (uint)Marshal.SizeOf(currentMonitorInfo);
                User32.GetMonitorInfo(currentMonitor, ref currentMonitorInfo);
                form.showAlert(currentLayout.ToString(), currentMonitorInfo.rcMonitor.X, currentMonitorInfo.rcMonitor.Width, currentMonitorInfo.rcMonitor.Y, currentMonitorInfo.rcMonitor.Height);
                DWM.Draw(currentPair);
                DWM.SaveLayouts();
            }
            if (e == 0x12) //enter
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                DWM.SetMainWindow(currentPair, selected);
                DWM.Draw(currentPair);
            }
            if (e == 0x15) // j
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                DWM.RotateFocusedWindowClockwise(currentPair, selected);
                DWM.Draw(currentPair);
            }
            if (e == 0x14) // k
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                DWM.RotateFocusedWindowCounterClockwise(currentPair, selected);
                DWM.Draw(currentPair);
            }
            if (e == 0x13) // l
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                DWM.MoveWindowClockwise(currentPair, selected);
                DWM.Draw(currentPair);
            }
            if (e == 0x16) //h
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                HMONITOR currentMonitor = User32.MonitorFromWindow(selectedWindow, User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                DesktopWindow selected = DWM.GetWindowByHandlers(selectedWindow, currentMonitor, VirtualDesktop.Current);
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                DWM.MoveWindowCounterClockwise(currentPair, selected);
                DWM.Draw(currentPair);
            }
            if (e == 0x17) //z
            {
                DWM.ClearWindows();
                DWM.GetWindows();
                DWM.Draw();
            }
            if (e == 0x18) //p
            {
                HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                VirtualDesktop currentDesktop = VirtualDesktop.Current;
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                DWM.RotateMonitorClockwise(currentPair);
            }
            if (e == 0x19) //n
            {
                HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                VirtualDesktop currentDesktop = VirtualDesktop.Current;
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                DWM.RotateMonitorCounterClockwise(currentPair);
            }
            if (e == 0x21) //space bar
            {
                HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                VirtualDesktop currentDesktop = VirtualDesktop.Current;
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                AlertForm form = new AlertForm();
                Layout currentLayout = DWM.RotateLayoutCounterClockwise(currentPair);
                User32.MONITORINFO currentMonitorInfo = new User32.MONITORINFO();
                currentMonitorInfo.cbSize = (uint)Marshal.SizeOf(currentMonitorInfo);
                User32.GetMonitorInfo(currentMonitor, ref currentMonitorInfo);
                form.showAlert(currentLayout.ToString(), currentMonitorInfo.rcMonitor.X, currentMonitorInfo.rcMonitor.Width, currentMonitorInfo.rcMonitor.Y, currentMonitorInfo.rcMonitor.Height);
                DWM.Draw(currentPair);
                DWM.SaveLayouts();
            }
            if (e == 0x22) //h
            {
                debounceDispatcher.Debounce(() =>
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    DWM.ExpandMainPane(currentPair);
                    DWM.Draw(currentPair);
                    DWM.SaveFactors();
                });
            }
            if (e == 0x23) //l
            {
                debounceDispatcher.Debounce(() =>
                {
                    HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                    Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(VirtualDesktop.Current, currentMonitor);
                    DWM.ShrinkMainPane(currentPair);
                    DWM.Draw(currentPair);
                    DWM.SaveFactors();
                });
            }
            if (e == 0x24) //j
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                DesktopWindow selected = DWM.FindWindow(selectedWindow);
                DWM.MoveWindowPreviousScreen(selected);
                DWM.Draw();
            }
            if (e == 0x25) //k
            {
                HWND selectedWindow = User32.GetForegroundWindow();
                DesktopWindow selected = DWM.FindWindow(selectedWindow);
                DWM.MoveWindowNextScreen(selected);
                DWM.Draw();
            }
            if (e == 0x21) //l
            {
                HMONITOR currentMonitor = User32.MonitorFromWindow(User32.GetForegroundWindow(), User32.MonitorFlags.MONITOR_DEFAULTTONEAREST);
                VirtualDesktop currentDesktop = VirtualDesktop.Current;
                Pair<VirtualDesktop, HMONITOR> currentPair = new Pair<VirtualDesktop, HMONITOR>(currentDesktop, currentMonitor);
                DWM.RotateLayoutCounterClockwise(currentPair);
                DWM.Draw(currentPair);
                DWM.SaveLayouts();
            }
            if (e == 0x26) //right
            {
                debounceDispatcher.Debounce(() =>
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = DWM.FindWindow(selectedWindow);
                    DWM.MoveWindowNextVirtualDesktop(selected);
                    DWM.Draw();
                });
            }
            if (e == 0x27) //right
            {
                debounceDispatcher.Debounce(() =>
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = DWM.FindWindow(selectedWindow);
                    DWM.MoveWindowPreviousVirtualDesktop(selected);
                    DWM.Draw();
                });
            }
            if (e == 0x1 || e == 0x2 || e == 0x3 || e == 0x4 || e == 0x5) //1,2,3,4,5
            {
                debounceDispatcher.Debounce(() =>
                {
                    HWND selectedWindow = User32.GetForegroundWindow();
                    DesktopWindow selected = DWM.FindWindow(selectedWindow);
                    DWM.MoveWindowSpecificVirtualDesktop(selected, e - 1);
                    DWM.Draw();
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
            message.Add("disabled_read", DWM.Disabled);
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
                    Screen screen = System.Windows.Forms.Screen.AllScreens.First(s => s.Bounds == new Rectangle(w.value.Monitor.rcMonitor.X, w.value.Monitor.rcMonitor.Y, w.value.Monitor.rcMonitor.Width, w.value.Monitor.rcMonitor.Height));


                    List<String> item = new List<string>();
                    item.Add(w.value.Window.ToString());
                    item.Add(w.value.AppName);
                    item.Add(w.value.ClassName);
                    item.Add(w.value.VirtualDesktop.ToString());
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
