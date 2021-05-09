using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

        private async void MainPage_SendFilters()
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
            this.MainPage_Refresh();
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
                                w[6],
                                w[7]
                        ));
                    }

                    App.mainViewModel.DesktopWindows = windowsReceived;
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

                    App.mainViewModel.Filters = filtersReceived;
                }

                if (args.Request.Message.ContainsKey("padding_read"))
                {
                    args.Request.Message.TryGetValue("padding_read", out object message);
                    App.mainViewModel.Padding = int.Parse(message.ToString());
                }
            }
        }

        private void FilterWindowsButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            DesktopWindow desktopWindow = (DesktopWindow)item;
            List<Filter> filters = new List<Filter>(App.mainViewModel.Filters);

            Filter filter = new Filter(desktopWindow.AppName);
            if (!filters.Contains(filter))
            {
                filters.Add(filter);
            }
 
            App.mainViewModel.Filters = filters;
            MainPage_SendFilters();
        }

        private void FilterClassButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            DesktopWindow desktopWindow = (DesktopWindow)item;
            List<Filter> filters = new List<Filter>(App.mainViewModel.Filters);

            Filter filter = new Filter(desktopWindow.AppName, desktopWindow.ClassName);
            if (!filters.Contains(filter))
            {
                filters.Add(filter);
            }

            App.mainViewModel.Filters = filters;
            MainPage_SendFilters();
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
