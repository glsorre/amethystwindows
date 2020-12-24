using AmethystWindows.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        private MainViewModel mainViewModel = new MainViewModel();

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = mainViewModel;
            ApplicationsBarButton.IsChecked = true;
            App.AppServiceConnected += MainPage_AppServiceConnected;
        }

        private async void MainPage_AppServiceConnected(object sender, AppServiceTriggerDetails e)
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
                    if (App.IsForeground)
                    {
                        byte[] bytes = Encoding.ASCII.GetBytes(message.ToString());
                        ReadOnlySpan<byte> readOnlySpan = new ReadOnlySpan<byte>(bytes);
                        List<List<string>> windowsParsed = JsonSerializer.Deserialize<List<List<string>>>(readOnlySpan);
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

                        mainViewModel.DesktopWindows = windowsReceived;
                    }
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
