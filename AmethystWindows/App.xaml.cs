using AmethystWindows.ViewModels;
using GalaSoft.MvvmLight.Threading;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AmethystWindows
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static BackgroundTaskDeferral AppServiceDeferral = null;
        public static AppServiceConnection Connection = null;
        public static event EventHandler AppServiceDisconnected;
        public static event EventHandler<AppServiceTriggerDetails> AppServiceConnected;
        public static bool IsForeground = true;

        public static MainViewModel mainViewModel = new MainViewModel();
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            AppCenter.Start("d37be467-14e5-48f8-b6be-42080bc64dc9", typeof(Analytics), typeof(Crashes));
            Suspending += App_OnSuspending;
            LeavingBackground += App_LeavingBackground;
            EnteredBackground += App_EnteredBackground;
            Windows.UI.ViewManagement.ApplicationView.PreferredLaunchViewSize = new Windows.Foundation.Size(1000, 500);
            Windows.UI.ViewManagement.ApplicationView.PreferredLaunchWindowingMode = Windows.UI.ViewManagement.ApplicationViewWindowingMode.PreferredLaunchViewSize;
            AppServiceConnected += App_AppServiceConnected;
        }

        public async void App_LaunchSystray()
        {
            if (ApiInformation.IsApiContractPresent(
                            "Windows.ApplicationModel.FullTrustAppContract", 1, 0))
            {
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
        }

        private void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            IsForeground = false;
        }

        private void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            IsForeground = true;
        }

        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Connection.ServiceClosed -= Connection_ServiceClosed;
            Connection = null;
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);
            if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails details)
            {
                if (details.CallerPackageFamilyName == Package.Current.Id.FamilyName)
                {
                    AppServiceDeferral = args.TaskInstance.GetDeferral();
                    args.TaskInstance.Canceled += Connection_OnTaskCanceled;
                    Connection = details.AppServiceConnection;
                    Connection.RequestReceived += Connection_RequestReceived;
                    Connection.ServiceClosed += Connection_ServiceClosed;
                    AppServiceConnected?.Invoke(this, args.TaskInstance.TriggerDetails as AppServiceTriggerDetails);
                }
            }
        }

        private void Connection_OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (AppServiceDeferral != null)
            {
                AppServiceDeferral.Complete();
            }
            AppServiceDisconnected?.Invoke(this, null);
        }

        private async void App_AppServiceConnected(object sender, AppServiceTriggerDetails e)
        {
            //Connection.RequestReceived += Connection_RequestReceived;
            await Task.Delay(500);
            App_Refresh();
        }

        public static async void App_Refresh()
        {
            ValueSet message = new ValueSet();
            message.Add("refresh", "");
            await Connection.SendMessageAsync(message);
        }

        public static async void App_SendFilters()
        {
            ValueSet message = new ValueSet();
            List<List<String>> list = new List<List<String>>();
            foreach (var f in App.mainViewModel.Filters)
            {
                List<String> item = new List<string>();
                item.Add(f.AppName);
                item.Add(f.ClassName);
                list.Add(item);
            }
            message.Add("filters_set", JsonConvert.SerializeObject(list));
            await Connection.SendMessageAsync(message);
        }

        private void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            if (IsForeground)
            {
                if (args.Request.Message.ContainsKey("refresh"))
                {
                    args.Request.Message.TryGetValue("refresh", out object message);
                    List<List<string>> windowsParsed = JsonConvert.DeserializeObject<List<List<string>>>(message.ToString());
                    List<DesktopWindow> windowsReceived = new List<DesktopWindow>();

                    foreach (List<string> w in windowsParsed)
                    {
                        windowsReceived.Add(
                            new DesktopWindow(
                                w[0],
                                w[1],
                                w[2],
                                w[3],
                                w[4],
                                w[5],
                                w[6],
                                w[7]
                        ));
                    }

                    mainViewModel.DesktopWindows = windowsReceived;
                }

                if (args.Request.Message.ContainsKey("filters_read"))
                {
                    args.Request.Message.TryGetValue("filters_read", out object message);
                    List<List<string>> filtersParsed = JsonConvert.DeserializeObject<List<List<string>>>(message.ToString());
                    List<Filter> filtersReceived = new List<Filter>();

                    foreach (List<string> f in filtersParsed)
                    {
                        filtersReceived.Add(
                            new Filter(
                                f[0],
                                f[1]
                        ));
                    }

                    mainViewModel.Filters = filtersReceived;
                }

                if (args.Request.Message.ContainsKey("padding_read"))
                {
                    args.Request.Message.TryGetValue("padding_read", out object message);
                    mainViewModel.Padding = int.Parse(message.ToString());
                }

                if (args.Request.Message.ContainsKey("margin_top_read"))
                {
                    args.Request.Message.TryGetValue("margin_top_read", out object message);
                    mainViewModel.MarginTop = int.Parse(message.ToString());
                }

                if (args.Request.Message.ContainsKey("margin_bottom_read"))
                {
                    args.Request.Message.TryGetValue("margin_bottom_read", out object message);
                    mainViewModel.MarginBottom = int.Parse(message.ToString());
                }

                if (args.Request.Message.ContainsKey("margin_left_read"))
                {
                    args.Request.Message.TryGetValue("margin_left_read", out object message);
                    mainViewModel.MarginLeft = int.Parse(message.ToString());
                }

                if (args.Request.Message.ContainsKey("margin_right_read"))
                {
                    args.Request.Message.TryGetValue("margin_right_read", out object message);
                    mainViewModel.MarginRight = int.Parse(message.ToString());
                }

                if (args.Request.Message.ContainsKey("layout_padding_read"))
                {
                    args.Request.Message.TryGetValue("layout_padding_read", out object message);
                    mainViewModel.LayoutPadding = int.Parse(message.ToString());
                }

                if (args.Request.Message.ContainsKey("exit"))
                {
                    App.Current.Exit();
                }
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            App_LaunchSystray();
            IsForeground = true;
            DispatcherHelper.Initialize();
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += RootFrame_OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }
                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            App_LaunchSystray();
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                Window.Current.Content = rootFrame;
            }

            string payload = string.Empty;
            if (args.Kind == ActivationKind.StartupTask)
            {
                var startupArgs = args as StartupTaskActivatedEventArgs;
                payload = ActivationKind.StartupTask.ToString();
            }

            rootFrame.Navigate(typeof(MainPage), payload);
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void RootFrame_OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void App_OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            if (AppServiceDeferral != null)
            {
                AppServiceDeferral.Complete();
            }
            deferral.Complete();
        }

    }
}
