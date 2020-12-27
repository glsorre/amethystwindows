using AmethystWindows.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AmethystWindows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            DataContext = App.mainViewModel;
            ApplicationsBarButton.IsChecked = true;
            SettingsBarButton.Click += SettingsBarButton_Click;
            RefreshButton.Click += RefreshButton_Click;
            RedrawButton.Click += RedrawButton_Click;
            App.AppServiceConnected += MainPage_AppServiceConnected;
        }

        private async void RedrawButton_Click(object sender, RoutedEventArgs e)
        {
            ValueSet message = new ValueSet();
            message.Add("redraw", "");
            await App.Connection.SendMessageAsync(message);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage_Refresh();
        }

        private async void MainPage_Refresh()
        {
            ValueSet message = new ValueSet();
            message.Add("refresh", "");
            await App.Connection.SendMessageAsync(message);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter.ToString() == "SettingPage")
            {
                MainPage_Refresh();
            }
        }

        private void SettingsBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingPage), "MainPage");
        }

        private void MainPage_AppServiceConnected(object sender, AppServiceTriggerDetails e)
        {
            App.Connection.RequestReceived += Connection_RequestReceived;
        }

        private void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            if (App.IsForeground)
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
                                w[6]
                        ));
                    }

                    App.mainViewModel.DesktopWindows = windowsReceived;
                }

                if (args.Request.Message.ContainsKey("padding_read"))
                {
                    args.Request.Message.TryGetValue("padding_read", out object message);
                    App.mainViewModel.Padding = int.Parse(message.ToString());
                }
            }
        }
    }

    public sealed class AppBarLockableToggleButton : AppBarToggleButton
    {
        protected override void OnToggle()
        {
            if (!LockToggle)
            {
                base.OnToggle();
            }
        }

        public bool LockToggle { get; set; }
    }
}
